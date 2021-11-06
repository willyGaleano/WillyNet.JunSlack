using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Enums;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Infraestructure.Persistence.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        }
    }
}
