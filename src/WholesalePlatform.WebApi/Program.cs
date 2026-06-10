using Microsoft.AspNetCore.DataProtection;
using WholesalePlatform.Application;
using WholesalePlatform.Infrastructure;
using WholesalePlatform.WebApi.Extensions;
using WholesalePlatform.WebApi.Logging;

if (IsEnvironmentUnset() && IsDebugBuildOutput())
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
}

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var keysPath = Path.Combine(builder.Environment.ContentRootPath, "data-protection-keys");
    builder.Services
        .AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath));
}

builder.Logging.AddColoredConsoleLogger(
    LogLevel.Information,
    options =>
    {
        options.SetCategoryMinimumLevel("Microsoft.AspNetCore", LogLevel.Warning);
        options.SetCategoryMinimumLevel("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
    });

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", app.Environment.IsDevelopment()))
{
    await app.ApplyDatabaseMigrationsAsync();
}

await app.SeedAdministratorAsync();

app.UseWebApiPipeline();
app.MapControllers();

app.Run();

static bool IsEnvironmentUnset()
{
    return string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
        && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"));
}

static bool IsDebugBuildOutput()
{
    var baseDirectory = AppContext.BaseDirectory;

    return baseDirectory.Contains(@"\bin\Debug\", StringComparison.OrdinalIgnoreCase)
        || baseDirectory.Contains("/bin/Debug/", StringComparison.OrdinalIgnoreCase);
}
