using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Exceptions;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Application.Features.Channels.Commands
{
    public class EditChannelCommand : IRequest<Response<Guid>>
    {
        public Guid ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelType ChannelType { get; set; }
    }

    public class EditHandler : IRequestHandler<EditChannelCommand, Response<Guid>>
    {
        private readonly IRepositoryGenericSpecification<Channel> _repository;

        public EditHandler(IRepositoryGenericSpecification<Channel> repository)
        {
            _repository = repository;
        }
        public async Task<Response<Guid>> Handle(EditChannelCommand request, CancellationToken cancellationToken)
        {
            var channel = await _repository.GetByIdAsync(request.ChannelId, cancellationToken);
            if(channel == null)
                throw new ApiException("El canal no existe.");

            channel.ChannelType = request.ChannelType;
            channel.Name = request.Name ?? channel.Name;
            channel.Description = request.Description ?? channel.Description;

            try
            {
                await _repository.UpdateAsync(channel, cancellationToken);
                return new Response<Guid>(channel.ChannelId, "Se editó correctamente el canal.");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
