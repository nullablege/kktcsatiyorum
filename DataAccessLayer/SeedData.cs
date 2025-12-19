using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<UygulamaKullanicisi>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // User
            var adminEmail = "admin@site.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new UygulamaKullanicisi
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    AdSoyad = "Sistem Yöneticisi"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(" | ",
                        result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}