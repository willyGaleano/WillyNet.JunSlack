using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.Channels.Specifications
{
    public class GetChannelSpecification : Specification<Channel>, ISingleResultSpecification
    {
        public GetChannelSpecification(Guid channelId)
        {
            Query.Where(x => x.ChannelId == channelId);
            Query.Include(x => x.Messages)
                .ThenInclude(x => x.Sender);
        }
    }
}
