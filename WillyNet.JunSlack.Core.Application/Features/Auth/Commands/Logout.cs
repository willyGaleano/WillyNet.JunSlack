using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;

namespace WillyNet.JunSlack.Core.Application.Features.Auth.Commands
{
    public class Logout : IRequest<Response<bool>>
    {
        public string UserId { get; set; }
    }
    public class LogoutHandler : IRequestHandler<Logout, Response<bool>>
    {
        private readonly IAccountService _accountService;

        public LogoutHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<Response<bool>> Handle(Logout request, CancellationToken cancellationToken)
        {
            return await _accountService.Logout(request.UserId);
        }
    }
}
