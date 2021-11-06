using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs.Account;
using WillyNet.JunSlack.Core.Application.Enums;
using WillyNet.JunSlack.Core.Application.Exceptions;
using WillyNet.JunSlack.Core.Application.Features.RefreshTokens.Specifications;
using WillyNet.JunSlack.Core.Application.Helpers;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;
using WillyNet.JunSlack.Core.Domain.Settings;
using WillyNet.JunSlack.Infraestructure.Persistence.Contexts;

namespace WillyNet.JunSlack.Infraestructure.Persistence.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IDateTimeService _dateTimeService;
        private readonly IRepositoryGenericSpecification<RefreshToken> _repositoryRefTok;
        private readonly ApplicationDbContext _dbContext;
        public AccountService(UserManager<AppUser> userManager,
            IOptions<JWTSettings> jwtSettings,
            IDateTimeService dateTimeService,
            SignInManager<AppUser> signInManager,
            IRepositoryGenericSpecification<RefreshToken> repositoryRefTok,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _dateTimeService = dateTimeService;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _repositoryRefTok = repositoryRefTok;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }
            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = new()
            {
                Id = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName
            };
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;

            var refrtoq = await _repositoryRefTok.ListAsync(new GetAllRefTokByIUserIdSpecification(user.Id));

            if (refrtoq != null && refrtoq.Count > 0 && refrtoq.Any(a => a.IsActive))
            {
                var activeRefreshToken = refrtoq.Where(a => a.IsActive == true).FirstOrDefault();
                response.RefreshToken = activeRefreshToken.Token;
                response.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = GenerateRefreshToken(ipAddress);
                response.RefreshToken = refreshToken.Token;
                response.RefreshTokenExpiration = refreshToken.Expires;

                refreshToken.UserAppId = user.Id;
                refreshToken.RefreshTokenId = Guid.NewGuid();
                await _repositoryRefTok.AddAsync(refreshToken);
            }

            return new Response<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.");
            }
            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.UserName
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                    //var verificationUri = await SendVerificationEmail(user, origin);
                    //TODO: Attach Email Service here and configure it via appsettings
                    //await _emailService.SendAsync(new Application.DTOs.Email.EmailRequest() { From = "mail@codewithmukesh.com", To = user.Email, Body = $"Please confirm your account by visiting this URL {verificationUri}", Subject = "Confirm Registration" });
                    return new Response<string>(user.Id, message: "User Registered");
                }
                else
                {
                    throw new ApiException($"{result.Errors.ToArray()[0].Description}");
                }
            }
            else
            {
                throw new ApiException($"Email {request.Email } is already registered.");
            }
        }

        private async Task<JwtSecurityToken> GenerateJWToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private static string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private static RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedToken = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        public async Task<Response<AuthenticationResponse>> RefreshTokenAsync(string refToken, string ipAddress)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refToken));
            if (user == null)
                return new Response<AuthenticationResponse>("El token no coincide con ningun usuario");

            var refreshToken = await _repositoryRefTok.GetBySpecAsync(new GetRefTokByTokenSpecification(refToken));
            if (!refreshToken.IsActive)
                return new Response<AuthenticationResponse>("El token no está activo");

            refreshToken.Revoked = DateTime.Now;
            await _repositoryRefTok.UpdateAsync(refreshToken);

            var newRefreshToken = GenerateRefreshToken(ipAddress);
            newRefreshToken.UserAppId = user.Id;
            newRefreshToken.RefreshTokenId = Guid.NewGuid();
            await _repositoryRefTok.AddAsync(newRefreshToken);

            //Generando un nuevo jwt
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = new()
            {
                Id = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName
            };
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.RefreshToken = newRefreshToken.Token;
            response.RefreshTokenExpiration = newRefreshToken.Expires;
            return new Response<AuthenticationResponse>(response, $"RefreshToken {user.UserName}");
        }

        public async Task<Response<bool>> RevokeToken(string token)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null) return new Response<bool>(false);

            var refreshToken = await _repositoryRefTok.GetBySpecAsync(new GetRefTokByTokenSpecification(token));
            if (!refreshToken.IsActive) return new Response<bool>(false);

            refreshToken.Revoked = DateTime.Now;
            await _repositoryRefTok.UpdateAsync(refreshToken);
            return new Response<bool>(true);
        }

        public async Task<Response<bool>> Logout(string idUser)
        {
            var user = await _userManager.FindByIdAsync(idUser);
            if (user == null)
                throw new ApiException("No existe el usuario.");

            if (!user.IsOnline) return new Response<bool>(false, "La sesión ya fue cerrada.");

            user.IsOnline = false;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApiException("No se pudo cerrar la sesión.");

            return new Response<bool>(true, "Sesión cerrada corectamente");
        }

        /*
       private async Task<string> SendVerificationEmail(AppUser user, string origin)
       {
           var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
           code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
           var route = "api/account/confirm-email/";
           var _enpointUri = new Uri(string.Concat($"{origin}/", route));
           var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
           verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
           //Email Service Call Here
           return verificationUri;
       }

       public async Task<Response<string>> ConfirmEmailAsync(string userId, string code)
       {
           var user = await _userManager.FindByIdAsync(userId);
           code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
           var result = await _userManager.ConfirmEmailAsync(user, code);
           if (result.Succeeded)
           {
               return new Response<string>(user.Id, message: $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
           }
           else
           {
               throw new ApiException($"An error occured while confirming {user.Email}.");
           }
       }




       public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
       {
           var account = await _userManager.FindByEmailAsync(model.Email);

           // always return ok response to prevent email enumeration
           if (account == null) return;

           var code = await _userManager.GeneratePasswordResetTokenAsync(account);
           var route = "api/account/reset-password/";
           var _enpointUri = new Uri(string.Concat($"{origin}/", route));
           var emailRequest = new EmailRequest()
           {
               Body = $"You reset token is - {code}",
               To = model.Email,
               Subject = "Reset Password",
           };
           await _emailService.SendAsync(emailRequest);
       }

       public async Task<Response<string>> ResetPassword(ResetPasswordRequest model)
       {
           var account = await _userManager.FindByEmailAsync(model.Email);
           if (account == null) throw new ApiException($"No Accounts Registered with {model.Email}.");
           var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);
           if (result.Succeeded)
           {
               return new Response<string>(model.Email, message: $"Password Resetted.");
           }
           else
           {
               throw new ApiException($"Error occured while reseting the password.");
           }
       }*/
    }
}
