using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs;
using WillyNet.JunSlack.Core.Application.Exceptions;
using WillyNet.JunSlack.Core.Application.Features.Channels.Specifications;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Application.Features.Channels.Queries
{
    public class GetPrivateChannel : IRequest<Response<ChannelDto>>
    {
        public string UserId { get; set; }
    }

    public class GetPrivateChannelHandler : IRequestHandler<GetPrivateChannel, Response<ChannelDto>>
    {
        private readonly IRepositoryGenericSpecification<Channel> _repository;
        private readonly IAuthenticatedUserService _currentUser;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public GetPrivateChannelHandler(IRepositoryGenericSpecification<Channel> repository, IMapper mapper,
                IAuthenticatedUserService currentUser
            )
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
        }
        public async Task<Response<ChannelDto>> Handle(GetPrivateChannel request, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.FindByIdAsync(_currentUser.UserId);
            var user = await _userManager.FindByIdAsync(request.UserId);

            var privateChannelIdForCurrentUser = GetPrivateChannelId(currentUser.Id, user.Id);
            var privateChannelIdForRecipientUser = GetPrivateChannelId(user.Id, currentUser.Id);

            var channel = await _repository.GetBySpecAsync(
                 new GetPrivateChannelSpecification(privateChannelIdForCurrentUser, privateChannelIdForRecipientUser)  
                 , cancellationToken);
            if(channel == null)
            {
                var newChannel = new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = currentUser.UserName,
                    Description = user.UserName,
                    ChannelType = ChannelType.Room,
                    PrivateChannelId = privateChannelIdForCurrentUser
                };

                var result = await _repository.AddAsync(newChannel, cancellationToken);
                if (result == null)
                    throw new ApiException("No se pudo devolver el canal.");

                var newChannelDto = _mapper.Map<ChannelDto>(newChannel);

                return new Response<ChannelDto>(newChannelDto, "Consulta exitosa.");
            }

            var channelDto = _mapper.Map<ChannelDto>(channel);

            return new Response<ChannelDto>(channelDto, "Consulta exitosa.");
        }

        private static string GetPrivateChannelId(string currentUserId, string userId) 
                        => $"{currentUserId}/{userId}";        
    }

}
