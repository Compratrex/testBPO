using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Application.Abstractions.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);
}

