using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using WholesalePlatform.Application.Abstractions.Clock;
using Microsoft.IdentityModel.Tokens;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Infrastructure.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private const string PermissionClaimType = "permission";
    private const string PermissionVersionClaimType = "permission_version";

    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions, IDateTimeProvider dateTimeProvider)
    {
        _jwtOptions = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(PermissionVersionClaimType, user.PermissionVersion.ToString())
        };

        claims.AddRange(user.Permissions.Select(permission =>
            new Claim(PermissionClaimType, permission.Permission.ToString())));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: _dateTimeProvider.UtcNow.UtcDateTime.AddMinutes(_jwtOptions.ExpiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
