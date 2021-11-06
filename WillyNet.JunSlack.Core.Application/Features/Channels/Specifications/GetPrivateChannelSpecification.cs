using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.Channels.Specifications
{
    class GetPrivateChannelSpecification : Specification<Channel>, ISingleResultSpecification
    {
        public GetPrivateChannelSpecification(string privIdCurrentUser, string privIdRecipientUser)
        {
            Query.Where(x => x.PrivateChannelId == privIdCurrentUser || x.PrivateChannelId == privIdRecipientUser );
            Query.Include(x => x.Messages)
                .ThenInclude(x => x.Sender);
        }
    }
}
