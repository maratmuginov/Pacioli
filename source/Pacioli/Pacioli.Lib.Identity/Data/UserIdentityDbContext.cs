using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pacioli.Lib.Identity.Models;
using Pacioli.Lib.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static Pacioli.Lib.Shared.Helpers.JsonHelper;

namespace Pacioli.Lib.Identity.Data
{
    public class UserIdentityDbContext : IdentityDbContext<User>
    {
        public UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options) : base(options)
        {

        }

        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = await File.ReadAllLinesAsync("roleNames.txt");
            await SeedUserRolesAsync(roleNames, roleManager);
            
            var seedUsers = await DeserializeFileAsync<List<RegisterModel>>("seedUsers.json");
            foreach (var seedUser in seedUsers)
            {
                if (await userManager.FindByNameAsync(seedUser.Username) is null &&
                    await userManager.FindByEmailAsync(seedUser.Email) is null)
                {
                    var user = new User {UserName = seedUser.Username, Email = seedUser.Email};
                    var identityResult = await userManager.CreateAsync(user, seedUser.Password);
                    if (identityResult.Succeeded)
                        await userManager.AddToRolesAsync(user, seedUser.RoleNames);
                }
            }
        }

        private static async Task SeedUserRolesAsync(IEnumerable<string> roleNames, RoleManager<IdentityRole> roleManager)
        {
            foreach (string roleName in roleNames)
                if (await roleManager.RoleExistsAsync(roleName) is false)
                    await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
