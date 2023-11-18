using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.IdentityContext
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Andrew Farouk",
                    Email = "andrewfarouk550@gmail.com",
                    PhoneNumber = "0123456789",
                    UserName = "andrewfarouk"
                };

                await userManager.CreateAsync(User, "Pa$$w0rd");
            }
        }
    }
}
