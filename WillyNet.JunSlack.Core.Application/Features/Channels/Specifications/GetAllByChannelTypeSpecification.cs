using Ardalis.Specification;
using System.Linq;
using WillyNet.JunSlack.Core.Domain.Entities;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Application.Features.Channels.Specifications
{
    public class GetAllByChannelTypeSpecification : Specification<Channel>
    {
        public GetAllByChannelTypeSpecification(ChannelType channelType)
        {
            Query.Where(x => x.ChannelType == channelType);           
        }
    }
}
