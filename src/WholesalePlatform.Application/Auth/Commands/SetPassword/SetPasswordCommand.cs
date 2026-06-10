using MediatR;

namespace WholesalePlatform.Application.Auth.Commands.SetPassword;

public sealed record SetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest;
