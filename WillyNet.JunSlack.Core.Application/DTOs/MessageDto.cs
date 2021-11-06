using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Application.DTOs
{
    public class MessageDto
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto Sender { get; set; }        
        public ChannelDto Channel { get; set; }        
        public MessageType MessageType { get; set; }
    }
}
