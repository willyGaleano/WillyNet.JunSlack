using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Domain.Common;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Domain.Entities
{
    public class Channel : AuditableBaseEntity
    {
        public Guid ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrivateChannelId { get; set; }
        public TypingNotification TypingNotification { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ChannelType ChannelType { get; set; }
    }
}
