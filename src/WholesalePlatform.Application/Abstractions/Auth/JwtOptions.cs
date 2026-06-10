namespace WholesalePlatform.Application.Abstractions.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public const int MinimumSecretBytes = 32;

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public int ExpiresMinutes { get; init; } = 30;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Issuer)
            && !string.IsNullOrWhiteSpace(Audience)
            && !string.IsNullOrWhiteSpace(Secret)
            && System.Text.Encoding.UTF8.GetByteCount(Secret) >= MinimumSecretBytes
            && ExpiresMinutes > 0;
    }
}
