using MediatR;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(
            request.Email,
            cancellationToken,
            includePermissions: true);

        if (user is null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid login or password.");
        }

        if (user.IsPasswordSetupRequired)
        {
            throw new UnauthorizedAccessException("Password setup is required before login.");
        }

        return new LoginResponseDto(_jwtTokenService.CreateToken(user));
    }
}
