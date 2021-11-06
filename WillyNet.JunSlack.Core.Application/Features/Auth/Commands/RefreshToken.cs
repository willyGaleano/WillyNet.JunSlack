using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs.Account;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;

namespace WillyNet.JunSlack.Core.Application.Features.Auth.Commands
{
    public class RefreshToken : IRequest<Response<AuthenticationResponse>>
    {
        public string RefToken { get; set; }
        public string IpAddress { get; set; }
    }

    public class RefreshTokenHandler : IRequestHandler<RefreshToken, Response<AuthenticationResponse>>
    {
        private readonly IAccountService _accountService;
        public RefreshTokenHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<Response<AuthenticationResponse>> Handle(RefreshToken request, CancellationToken cancellationToken)
        {
            return await _accountService.RefreshTokenAsync(request.RefToken, request.IpAddress);
        }
    }

}
