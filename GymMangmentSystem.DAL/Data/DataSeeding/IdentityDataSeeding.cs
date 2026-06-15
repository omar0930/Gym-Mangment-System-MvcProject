using GymMangmentSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool hasUsers = userManager.Users.Any();
                bool hasRoles = roleManager.Roles.Any();
                if (hasUsers && hasRoles) return;
                if (!hasRoles)

                {
                    var roles = new List<IdentityRole>
                    {
                        new IdentityRole() { Name = "SuperAdmin" },
                        new IdentityRole() { Name = "Admin" }
                    };
                    foreach (var roleName in roles.Select(R => R.Name))
                    {
                        if (!await roleManager.RoleExistsAsync(roleName!))
                        {
                            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName!));
                            if (!roleResult.Succeeded)
                            {
                                logger.LogError($"Failed to create role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                                return;
                            }
                        }
                    }
                }
                if (!hasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        UserName = "Omar25",
                        FirstName = "Omar",
                        LastName = "Mohamed",
                        PhoneNumber = "01012345678",
                        Email = "superadmin@example.com"
                    };
                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd!");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin01 = new ApplicationUser()
                    {
                        UserName = "Admin25",
                        FirstName = "Admin",
                        LastName = "Mohamed",
                        PhoneNumber = "01012345678",
                        Email = "admin@example.com"
                    };
                    var CreateResult = await userManager.CreateAsync(Admin01, "P@ssw0rd!");
                    if (!CreateResult.Succeeded)
                    {
                        logger.LogError($"Failed to create user {Admin01.UserName}: {string.Join(", ", CreateResult.Errors.Select(e => e.Description))}");
                        return;

                    }
                    logger.LogInformation($"Seed Admin01 {Admin01.Email} created successfully.");

                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred during identity data seeding: {ex.Message}");
                throw;
            }
        }
    }
}
