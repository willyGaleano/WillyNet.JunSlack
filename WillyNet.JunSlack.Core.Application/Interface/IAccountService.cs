using WillyNet.JunSlack.Core.Application.DTOs.Account;
using WillyNet.JunSlack.Core.Application.Wrappers;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WillyNet.JunSlack.Core.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
        Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);
        Task<Response<AuthenticationResponse>> RefreshTokenAsync(string jwtToken, string ipAddress);
        Task<Response<bool>> RevokeToken(string token);
        Task<Response<bool>> Logout(string idUser);
        //Task<Response<string>> ConfirmEmailAsync(string userId, string code);
        //Task ForgotPassword(ForgotPasswordRequest model, string origin);
        //Task<Response<string>> ResetPassword(ResetPasswordRequest model);
    }
}
