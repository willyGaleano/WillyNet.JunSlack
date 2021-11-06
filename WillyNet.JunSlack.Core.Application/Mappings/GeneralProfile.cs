using AutoMapper;
using WillyNet.JunSlack.Core.Application.DTOs;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Channel, ChannelDto>();
            CreateMap<Message, MessageDto>();
            CreateMap<TypingNotification, TypingNotificationDto>();
            CreateMap<AppUser, UserDto>();
        }
    }
}
