using MediatR;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Clock;
using WholesalePlatform.Application.Abstractions.Persistence;

namespace WholesalePlatform.Application.Auth.Commands.SetPassword;

public sealed class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SetPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null
            || !user.HasActivePasswordSetupToken(_dateTimeProvider.UtcNow)
            || user.PasswordSetupTokenHash is null
            || !_passwordHasher.VerifyPassword(user.PasswordSetupTokenHash, request.Token))
        {
            throw new UnauthorizedAccessException("Invalid or expired password setup token.");
        }

        user.CompletePasswordSetup(_passwordHasher.HashPassword(request.NewPassword));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
