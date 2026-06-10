using Microsoft.AspNetCore.Identity;
using WholesalePlatform.Application.Abstractions.Auth;

namespace WholesalePlatform.Infrastructure.Auth;

public sealed class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(string.Empty, password);
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(string.Empty, passwordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}

