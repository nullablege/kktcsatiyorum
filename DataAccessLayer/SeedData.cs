using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using EntityLayer.Constants;

namespace DataAccessLayer
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<UygulamaKullanicisi>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { RoleNames.Admin, RoleNames.User, RoleNames.Moderator };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@admin.com";

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
                    throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
            }
        }
    }
}