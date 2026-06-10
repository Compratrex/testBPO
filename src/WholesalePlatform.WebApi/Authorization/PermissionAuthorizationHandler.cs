using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.WebApi.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private const string PermissionClaimType = "permission";
    private const string PermissionVersionClaimType = "permission_version";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionAuthorizationHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var permissionVersionValue = context.User.FindFirstValue(PermissionVersionClaimType);

        if (!Guid.TryParse(userIdValue, out var userId)
            || !long.TryParse(permissionVersionValue, out var tokenPermissionVersion)
            || !Enum.TryParse<PermissionCode>(requirement.Permission, out var requiredPermission))
        {
            return;
        }

        var user = await _unitOfWork.Users.GetByIdAsync(
            userId,
            _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None,
            includePermissions: true);

        if (user is not null
            && user.PermissionVersion == tokenPermissionVersion
            && user.HasPermission(requiredPermission)
            && context.User.HasClaim(PermissionClaimType, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
