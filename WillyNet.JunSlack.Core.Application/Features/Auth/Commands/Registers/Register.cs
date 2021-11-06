using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs.Account;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;

namespace WillyNet.JunSlack.Core.Application.Features.Auth.Commands.Registers
{
    public class Register : IRequest<Response<string>>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Origin { get; set; }
    }

    public class RegisterHandler : IRequestHandler<Register, Response<string>>
    {
        private readonly IAccountService _accountService;

        public RegisterHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<Response<string>> Handle(Register request, CancellationToken cancellationToken)
        {
            return await _accountService.RegisterAsync(new RegisterRequest
            {
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                UserName = request.UserName,
            }, request.Origin);
        }        
    }

}
