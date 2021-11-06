using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Exceptions;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.TypingNotifications.Commands
{
    public class CreateTypingNotificationCommand : IRequest<Response<Guid>>
    {
        public Guid ChannelId { get; set; }
    }

    public class CreateHandler : IRequestHandler<CreateTypingNotificationCommand, Response<Guid>>
    {        
        private readonly IAuthenticatedUserService _currentUser;
        private readonly IRepositoryGenericSpecification<Channel> _repositoryChann;
        private readonly IRepositoryGenericSpecification<TypingNotification> _repositoryTy;
        private readonly UserManager<AppUser> _userManager;


        public CreateHandler(IRepositoryGenericSpecification<Channel> repositoryChann,
                IAuthenticatedUserService currentUser, IRepositoryGenericSpecification<TypingNotification> repositoryTy
            )
        {            
            _currentUser = currentUser;
            _repositoryChann = repositoryChann;
            _repositoryTy = repositoryTy;
        }

        public async Task<Response<Guid>> Handle(CreateTypingNotificationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.FindByIdAsync(_currentUser.UserId);
            var channel = await _repositoryChann.GetByIdAsync(request.ChannelId, cancellationToken);

            if (channel == null || currentUser == null)
                throw new ApiException("Canal no encontrado o el usuario no existe.");

            var typing = new TypingNotification
                    {
                        TypingNotificationId = Guid.NewGuid(),
                        Id = currentUser.Id,
                        ChannelId = channel.ChannelId
                    };

            var result = await _repositoryTy.AddAsync(typing, cancellationToken);
            if(result == null)
                throw new ApiException("Ocrrió un problema al guardar el typing");

            return new Response<Guid>(typing.TypingNotificationId, "Se guardó correctamente el typing.");
        }
    }
}
