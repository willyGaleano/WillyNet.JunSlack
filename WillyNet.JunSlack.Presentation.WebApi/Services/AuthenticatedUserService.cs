using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WillyNet.JunSlack.Core.Application.Interfaces;

namespace WillyNet.JunSlack.Presentation.WebApi.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
        }
        public string UserId { get; }
    }
}
