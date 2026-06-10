using Microsoft.EntityFrameworkCore;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Users;
using WholesalePlatform.Infrastructure.Persistence;

namespace WholesalePlatform.WebApi.Extensions;

public static class BootstrapAdminExtensions
{
    private const string BuiltInDevelopmentAdminEmail = "admin@example.com";
    private const string BuiltInDevelopmentAdminFullName = "System Administrator";
    private const string BuiltInDevelopmentAdminPassword = "Admin123456!";

    public static async Task SeedAdministratorAsync(this WebApplication app)
    {
        var section = app.Configuration.GetSection("BootstrapAdmin");
        var enabled = section.GetValue<bool>("Enabled");
        var useBuiltInDevelopmentAdmin = section.GetValue<bool>("UseBuiltInDevelopmentAdmin");
        var email = section["Email"];
        var password = section["Password"];
        var fullName = section["FullName"] ?? "System Administrator";

        if (!enabled)
        {
            return;
        }

        if (useBuiltInDevelopmentAdmin && !app.Environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "Built-in development administrator can only be enabled in Development environment.");
        }

        if (useBuiltInDevelopmentAdmin)
        {
            email = string.IsNullOrWhiteSpace(email) ? BuiltInDevelopmentAdminEmail : email;
            password = string.IsNullOrWhiteSpace(password) ? BuiltInDevelopmentAdminPassword : password;
            fullName = string.IsNullOrWhiteSpace(fullName) ? BuiltInDevelopmentAdminFullName : fullName;
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Bootstrap admin is enabled but BootstrapAdmin__Email or BootstrapAdmin__Password is missing.");
        }

        if (!useBuiltInDevelopmentAdmin && password.Length < 12)
        {
            throw new InvalidOperationException(
                "Bootstrap admin password must be at least 12 characters long.");
        }

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("BootstrapAdmin");

        var existingAdmin = await context.Users
            .IgnoreQueryFilters()
            .AnyAsync(user => user.Role == UserRole.Administrator);

        if (existingAdmin)
        {
            return;
        }

        var admin = User.CreateAdministrator(
            email,
            fullName,
            passwordHasher.HashPassword(password));

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

        logger.LogInformation("Bootstrap administrator {Email} was created.", email);

        if (useBuiltInDevelopmentAdmin)
        {
            logger.LogWarning(
                "Built-in development administrator is enabled. Disable BootstrapAdmin:UseBuiltInDevelopmentAdmin outside local demo environments.");
        }
    }
}
