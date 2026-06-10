using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Domain.Permissions;

public static class DefaultPermissions
{
    private static readonly IReadOnlySet<PermissionCode> CustomerPermissions =
        new HashSet<PermissionCode>
        {
            PermissionCode.ProductsRead,
            PermissionCode.OrdersReadOwn,
            PermissionCode.OrdersCreate,
            PermissionCode.OrdersCancelOwn
        };

    public static IReadOnlyCollection<PermissionCode> ForAdministrator()
    {
        return Enum.GetValues<PermissionCode>();
    }

    public static IReadOnlyCollection<PermissionCode> ForCustomer()
    {
        return CustomerPermissions.ToArray();
    }

    public static bool IsAllowedForCustomer(PermissionCode permission)
    {
        return CustomerPermissions.Contains(permission);
    }

    public static void EnsureAssignable(UserRole role, IEnumerable<PermissionCode> permissions)
    {
        if (role == UserRole.Administrator)
        {
            return;
        }

        var forbidden = permissions
            .Where(permission => !IsAllowedForCustomer(permission))
            .ToArray();

        if (forbidden.Length > 0)
        {
            throw new DomainException(
                $"Permissions are not allowed for customer role: {string.Join(", ", forbidden)}.");
        }
    }
}
