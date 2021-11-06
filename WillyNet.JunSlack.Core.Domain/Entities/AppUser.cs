using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WillyNet.JunSlack.Core.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string Avatar { get; set; }        
        public bool IsOnline { get; set; }        
        public string PrimaryAppColor { get; set; }
        public string SecondaryAppColor { get; set; }
        public TypingNotification TypingNotification { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
