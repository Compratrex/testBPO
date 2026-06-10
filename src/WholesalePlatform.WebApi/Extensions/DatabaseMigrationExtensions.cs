using Microsoft.EntityFrameworkCore;
using WholesalePlatform.Infrastructure.Persistence;

namespace WholesalePlatform.WebApi.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseMigration");

        logger.LogInformation("Applying database migrations.");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied.");
    }
}
