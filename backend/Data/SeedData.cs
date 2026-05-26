using instagram.Models;
using Microsoft.AspNetCore.Identity;
namespace instagram.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUsername = "admin_super";
            if (await userManager.FindByNameAsync(adminUsername) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = "admin@instagram.local",
                    Nume = "Admin",
                    Prenume = "Wowww",
                    DataNasterii = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "admin123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}