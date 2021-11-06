using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs.Account;
using WillyNet.JunSlack.Core.Application.Features.Auth.Commands;
using WillyNet.JunSlack.Core.Application.Features.Auth.Commands.Registers;

namespace WillyNet.JunSlack.Presentation.WebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        [HttpPost("Authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            var result = await Mediator.Send(new Authenticated
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GenerateIPAddress()
            });
            SetTokenCookie(result.Data.RefreshToken);
            return Ok(result);
        }

        [HttpGet("Logout/{id}")]
        public async Task<IActionResult> Logout(string id)
        {
            return Ok(await Mediator.Send(new Logout { UserId = id }));
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            //Request.Cookies.TryGetValue("X-refreshToken", out var refreshToken);
            var refreshToken = Request.Cookies["X-refreshToken"];
            var result = await Mediator.Send(new RefreshToken
            {
                RefToken = refreshToken,
                IpAddress = GenerateIPAddress()
            });
            if (!string.IsNullOrEmpty(result.Data?.RefreshToken))
                SetTokenCookie(result.Data.RefreshToken);
            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            return Ok(await Mediator.Send(new Register
            {
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                UserName = request.UserName,
                Origin = Request.Headers["origin"]
            }));
        }

        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            Response.Cookies.Append("X-refreshToken", token, cookieOptions);
        }
    }
}
