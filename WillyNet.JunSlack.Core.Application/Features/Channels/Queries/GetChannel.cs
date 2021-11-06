using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs;
using WillyNet.JunSlack.Core.Application.Features.Channels.Specifications;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.Channels.Queries
{
    public class GetChannel : IRequest<Response<ChannelDto>>
    {
        public Guid ChannelId { get; set; }
    }

    public class GetChannelHandler : IRequestHandler<GetChannel, Response<ChannelDto>>
    {
        private readonly IRepositoryGenericSpecification<Channel> _repository;
        private readonly IMapper _mapper;

        public GetChannelHandler(IRepositoryGenericSpecification<Channel> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Response<ChannelDto>> Handle(GetChannel request, CancellationToken cancellationToken)
        {
            var channel = await _repository.GetBySpecAsync(
                new GetChannelSpecification(request.ChannelId), cancellationToken
                );
            var channelDto = _mapper.Map<ChannelDto>(channel);
            return new Response<ChannelDto>(channelDto, "Consulta exitosa.");
        }
    }
}
