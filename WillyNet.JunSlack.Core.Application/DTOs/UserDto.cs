using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WillyNet.JunSlack.Core.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }        
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public bool IsOnline { get; set; }
        public string PrimaryAppColor { get; set; }
        public string SecondaryAppColor { get; set; }
        public ICollection<MessageDto> Messages { get; set; }
    }
}
