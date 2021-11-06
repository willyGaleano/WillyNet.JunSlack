using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WillyNet.JunSlack.Core.Domain.Common.Enumeration;

namespace WillyNet.JunSlack.Core.Application.DTOs
{
    public class ChannelDto
    {
        public Guid ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrivateChannelId { get; set; }
        public ICollection<MessageDto> Messages { get; set; }
        public ChannelType ChannelType { get; set; }
    }
}
