using System.Security.Cryptography;
using WholesalePlatform.Application.Abstractions.Auth;

namespace WholesalePlatform.Infrastructure.Auth;

public sealed class PasswordGenerator : IPasswordGenerator
{
    public string Generate()
    {
        Span<byte> bytes = stackalloc byte[18];
        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes)
            .Replace("+", "A", StringComparison.Ordinal)
            .Replace("/", "z", StringComparison.Ordinal)
            .TrimEnd('=');
    }
}

