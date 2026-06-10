using Microsoft.AspNetCore.Authorization;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.WebApi.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(PermissionCode permission)
    {
        Policy = PermissionAuthorizationPolicyProvider.PolicyPrefix + permission;
    }
}

