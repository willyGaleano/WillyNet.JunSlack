using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Exceptions;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.TypingNotifications.Commands
{
    public class DeleteTypingNotificationCommand : IRequest<Response<Guid>>
    {
        public Guid TypingNotificationId { get; set; }
    }

    public class DelteHandler : IRequestHandler<DeleteTypingNotificationCommand, Response<Guid>>
    {      
        private readonly IRepositoryGenericSpecification<TypingNotification> _repositoryTy;        


        public DelteHandler(IRepositoryGenericSpecification<TypingNotification> repositoryTy)
        {         
            _repositoryTy = repositoryTy;
        }
        public async Task<Response<Guid>> Handle(DeleteTypingNotificationCommand request, CancellationToken cancellationToken)
        {
            var typing = await _repositoryTy.GetByIdAsync(request.TypingNotificationId, cancellationToken);
            if (typing == null)
                throw new ApiException("No se encontró typing");

            await _repositoryTy.DeleteAsync(typing, cancellationToken);

            return new Response<Guid>(typing.Id);
        }
    }
}
