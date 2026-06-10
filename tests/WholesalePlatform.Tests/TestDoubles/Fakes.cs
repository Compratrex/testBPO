using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Clock;

namespace WholesalePlatform.Tests.TestDoubles;

internal sealed class FakeCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; init; }
    public string? Email { get; init; }
    public bool IsAuthenticated => UserId is not null;
}

internal sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; init; } = new(2026, 6, 10, 12, 0, 0, TimeSpan.Zero);
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return "hashed:" + password;
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        return passwordHash == HashPassword(password);
    }
}
