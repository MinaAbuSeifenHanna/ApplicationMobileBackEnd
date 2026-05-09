using Microsoft.AspNetCore.Identity;
using StayHub.Backend.Domain.Entities;
using StayHub.Backend.Domain.Enums;
using StayHub.Backend.Infrastructure.Data;

namespace StayHub.Backend.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Seed Roles
        string[] roles = { "Admin", "Host", "Client" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Admin User
        var adminEmail = "admin@stayhub.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                RoleType = UserRoleType.Admin,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Seed Amenities
        if (!context.Amenities.Any())
        {
            var amenities = new List<Amenity>
            {
                new Amenity { Name = "WiFi", IconName = "wifi" },
                new Amenity { Name = "Swimming Pool", IconName = "pool" },
                new Amenity { Name = "Air Conditioning", IconName = "ac" },
                new Amenity { Name = "Free Parking", IconName = "local_parking" },
                new Amenity { Name = "Kitchen", IconName = "kitchen" }
            };

            context.Amenities.AddRange(amenities);
            await context.SaveChangesAsync();
        }
    }
}
