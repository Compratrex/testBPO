using WholesalePlatform.Domain.Common;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Domain.Permissions;

public sealed class UserPermission : AuditableEntity
{
    private UserPermission()
    {
    }

    private UserPermission(Guid id, Guid userId, PermissionCode permission)
        : base(id)
    {
        UserId = userId;
        Permission = permission;
    }

    public Guid UserId { get; private set; }
    public PermissionCode Permission { get; private set; }

    public static UserPermission Create(Guid userId, PermissionCode permission)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("Permission user id cannot be empty.");
        }

        if (!Enum.IsDefined(permission))
        {
            throw new DomainException("Permission code is invalid.");
        }

        return new UserPermission(Guid.NewGuid(), userId, permission);
    }
}
