using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Enums;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Infraestructure.Persistence.Seeds
{
    public class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default Admin User
            var defaultUser = new AppUser
            {
                UserName = "Jun",
                Email = "sofia@gmail.com",        
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Holamundo123*");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                }
            }
        }
    }
}
