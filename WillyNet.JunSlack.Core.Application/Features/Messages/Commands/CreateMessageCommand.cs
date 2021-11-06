using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs;
using WillyNet.JunSlack.Core.Application.Exceptions;
using WillyNet.JunSlack.Core.Application.Interface;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Application.Features.Messages.Commands
{
    public class CreateMessageCommand : IRequest<Response<MessageDto>>
    {
        public Guid ChannelId { get; set; }
        public string Content { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public IFormFile File { get; set; }
    }

    public class CreateHandler : IRequestHandler<CreateMessageCommand, Response<MessageDto>>
    {
        private readonly IMapper _mapper;
        private readonly IMediaUpload _mediaUpload;
        private readonly IAuthenticatedUserService _currentUser;
        private readonly IRepositoryGenericSpecification<Channel> _repositoryChann;
        private readonly IRepositoryGenericSpecification<Message> _repositoryMessa;
        private readonly UserManager<AppUser> _userManager;


        public CreateHandler(IMediaUpload mediaUpload, IMapper mapper, IRepositoryGenericSpecification<Channel> repositoryChann,
                IAuthenticatedUserService currentUser, IRepositoryGenericSpecification<Message> repositoryMessa
            )
        {
            _mediaUpload = mediaUpload;
            _mapper = mapper;
            _currentUser = currentUser;
            _repositoryChann = repositoryChann;
            _repositoryMessa = repositoryMessa;
        }

        public async Task<Response<MessageDto>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.FindByIdAsync(_currentUser.UserId);
            var channel = await _repositoryChann.GetByIdAsync(request.ChannelId, cancellationToken);

            if (channel == null || currentUser == null)
                throw new ApiException("Canal no encontrado o el usuario no existe.");

            var newMessage = new Message
            {
                MessageId = Guid.NewGuid(),
                Content = request.MessageType == MessageType.Text
                                                        ? request.Content
                                                        : _mediaUpload.UploadMedia(request.File).Result.Url,
                ChannelId = request.ChannelId,
                SenderId = _currentUser.UserId,
                CreatedAt = DateTime.Now,
                MessageType = request.MessageType
            };

            var result = await _repositoryMessa.AddAsync(newMessage, cancellationToken);
            if(result == null)
                throw new ApiException("Ocrrió un problema al guardar el mensaje");

            var messageDto = _mapper.Map<MessageDto>(newMessage);

            return new Response<MessageDto>(messageDto, "Se creó correctamemte el mensaje");
        }
    }
}
