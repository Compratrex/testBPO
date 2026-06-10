using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Clock;
using WholesalePlatform.Application.Abstractions.Email;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Infrastructure.Auth;
using WholesalePlatform.Infrastructure.Email;
using WholesalePlatform.Infrastructure.Outbox;
using WholesalePlatform.Infrastructure.Persistence;

namespace WholesalePlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured. Use ConnectionStrings__DefaultConnection.");
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IPasswordGenerator, PasswordGenerator>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailSender, LoggingEmailSender>();
        services.AddScoped<IEmailOutbox, EmailOutbox>();
        services.AddHostedService<OutboxProcessor>();

        return services;
    }
}
