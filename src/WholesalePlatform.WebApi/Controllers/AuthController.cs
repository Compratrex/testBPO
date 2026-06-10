using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WholesalePlatform.Application.Auth.Commands.Login;
using WholesalePlatform.Application.Auth.Commands.SetPassword;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.WebApi.Contracts.Auth;
using WholesalePlatform.WebApi.Contracts.Common;
using WholesalePlatform.WebApi.Routing;
using Microsoft.AspNetCore.RateLimiting;

namespace WholesalePlatform.WebApi.Controllers;

[Route(ApiRoutes.Auth.Base)]
public sealed class AuthController : ApiControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost(ApiRoutes.Auth.Login)]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
        return OkResponse(response);
    }

    [HttpPost(ApiRoutes.Auth.SetPassword)]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> SetPassword(
        [FromBody] SetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new SetPasswordCommand(request.Email, request.Token, request.NewPassword),
            cancellationToken);

        return NoContent();
    }
}
