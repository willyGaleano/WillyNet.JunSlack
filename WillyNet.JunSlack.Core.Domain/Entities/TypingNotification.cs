using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Domain.Common;

namespace WillyNet.JunSlack.Core.Domain.Entities
{
    public class TypingNotification : AuditableBaseEntity
    {
        public Guid TypingNotificationId { get; set; }
        public string Id { get; set; }
        public AppUser Sender { get; set; }
        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; }        
    }
}
