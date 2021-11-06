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
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Application.Features.Channels.Queries.GetAll
{
    public class GetAllChannel : IRequest<Response<IEnumerable<ChannelDto>>>
    {
        public ChannelType ChannelType { get; set; } = ChannelType.Channel;
    }

    public class GetAllChannelHandler : IRequestHandler<GetAllChannel, Response<IEnumerable<ChannelDto>>>
    {
        private readonly IRepositoryGenericSpecification<Channel> _repository;
        private readonly IMapper _mapper;

        public GetAllChannelHandler(IRepositoryGenericSpecification<Channel> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Response<IEnumerable<ChannelDto>>> Handle(GetAllChannel request, CancellationToken cancellationToken)
        {
            var channel = await _repository.ListAsync(
                new GetAllByChannelTypeSpecification(request.ChannelType), cancellationToken);                                            

            var channelDto = _mapper.Map<IEnumerable<ChannelDto>>(channel);

            return new Response<IEnumerable<ChannelDto>>(channelDto, "Consulta exitosa.");
        }
    }
}
