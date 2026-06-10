using WholesalePlatform.Application.Abstractions.Clock;

namespace WholesalePlatform.Infrastructure.Auth;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

