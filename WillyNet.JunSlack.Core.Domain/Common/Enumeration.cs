using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WillyNet.JunSlack.Core.Domain.Common
{
    public class Enumeration
    {
        public enum MessageType
        {
            Text = 1,
            Media = 2
        }

        public enum ChannelType
        {
            Channel = 1,
            Room = 2,
            Starred = 3
        }
    }
}
