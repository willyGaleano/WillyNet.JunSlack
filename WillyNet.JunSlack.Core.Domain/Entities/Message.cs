using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Domain.Common;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Domain.Entities
{
    public class Message : AuditableBaseEntity
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public AppUser Sender { get; set; }
        public string SenderId { get; set; }        
        public Channel Channel { get; set; }
        public Guid ChannelId { get; set; }
        public MessageType MessageType { get; set; }
    }
}
