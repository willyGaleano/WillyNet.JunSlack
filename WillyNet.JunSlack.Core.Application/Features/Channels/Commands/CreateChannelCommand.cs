using FluentValidation;
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
    public class CreateChannelCommand : IRequest<Response<Guid>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelType ChannelType { get; set; } = ChannelType.Channel;
    }

    public class CommandValidator : AbstractValidator<CreateChannelCommand>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }

    public class CreateHandler : IRequestHandler<CreateChannelCommand, Response<Guid>>
    {
        private readonly IRepositoryGenericSpecification<Channel> _repository;

        public CreateHandler(IRepositoryGenericSpecification<Channel> repository)
        {
            _repository = repository;
        }

        public async Task<Response<Guid>> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            var result = await _repository.AddAsync(
                new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    ChannelType = request.ChannelType
                }, cancellationToken
                );
            if (result == null)
                throw new ApiException("No se puso crear el canal.");
            return new Response<Guid>(result.ChannelId, "Canal creado correctamente.");
        }
    }
}
