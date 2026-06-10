using MediatR;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;

