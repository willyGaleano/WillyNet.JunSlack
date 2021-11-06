using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs.Account;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;

namespace WillyNet.JunSlack.Core.Application.Features.Auth.Commands
{
    public class Authenticated : IRequest<Response<AuthenticationResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }
    public class AuthenticateHandler : IRequestHandler<Authenticated, Response<AuthenticationResponse>>
    {
        private readonly IAccountService _accountService;

        public AuthenticateHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<Response<AuthenticationResponse>> Handle(Authenticated request, CancellationToken cancellationToken)
        {
            return await _accountService.AuthenticateAsync(
                new AuthenticationRequest
                {
                    Email = request.Email,
                    Password = request.Password
                }, request.IpAddress);
        }
    }

}
