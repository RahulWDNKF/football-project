using FootballDashboardAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace FootballDashboardAPI.Services;

public static class IdentitySeedService
{
    private static readonly string[] Roles = ["Admin", "Player", "Scout"];

    public static async System.Threading.Tasks.Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("IdentitySeedService");

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create role '{role}': {errors}");
                }
            }
        }

        var enabled = configuration.GetValue<bool>("AdminSeed:Enabled");
        if (!enabled)
        {
            logger.LogInformation("Admin seed is disabled.");
            return;
        }

        var email = configuration["AdminSeed:Email"];
        var password = configuration["AdminSeed:Password"];
        var fullName = configuration["AdminSeed:FullName"] ?? "System Administrator";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Admin seed is enabled, but AdminSeed:Email or AdminSeed:Password is missing.");
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            if (!await userManager.IsInRoleAsync(existingUser, "Admin"))
            {
                await userManager.AddToRoleAsync(existingUser, "Admin");
            }

            if (!existingUser.IsActive || !existingUser.IsInviteAccepted || existingUser.Role != "Admin")
            {
                existingUser.IsActive = true;
                existingUser.IsInviteAccepted = true;
                existingUser.EmailConfirmed = true;
                existingUser.Role = "Admin";
                existingUser.FullName = string.IsNullOrWhiteSpace(existingUser.FullName) ? fullName : existingUser.FullName;
                await userManager.UpdateAsync(existingUser);
            }

            logger.LogInformation("Admin seed user already exists: {Email}", email);
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            Role = "Admin",
            IsActive = true,
            IsInviteAccepted = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(adminUser, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create admin seed user '{email}': {errors}");
        }

        var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
        if (!addToRoleResult.Succeeded)
        {
            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to add admin role to '{email}': {errors}");
        }

        logger.LogInformation("Admin seed user created: {Email}", email);
    }
}
