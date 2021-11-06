using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Exceptions;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.Users.Commands
{
    public class EditUserCommand : IRequest<Response<bool>>
    {
        public string UserId { get; set; }
        public string PrimaryAppColor { get; set; }
        public string SecondaryAppColor { get; set; }
    }

    public class EditHandler : IRequestHandler<EditUserCommand, Response<bool>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthenticatedUserService _currentService;

        public EditHandler(IAuthenticatedUserService currentService, UserManager<AppUser> userManager)
        {
            _currentService = currentService;
            _userManager = userManager;
        }

        public async Task<Response<bool>> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new ApiException("No existe el usuario.");

            user.PrimaryAppColor = request.PrimaryAppColor;
            user.SecondaryAppColor = request.SecondaryAppColor;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApiException("No se pudo editar el usuario.");

            return new Response<bool>(true, "Se editó correctamente el usuario.");
        }
    }
}
