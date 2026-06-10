using WholesalePlatform.Domain.Common;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Permissions;

namespace WholesalePlatform.Domain.Users;

public sealed class User : AuditableEntity
{
    public const int MaxEmailLength = 320;
    public const int MaxFullNameLength = 200;

    private readonly List<UserPermission> _permissions = [];

    private User()
    {
    }

    private User(
        Guid id,
        string email,
        string fullName,
        UserRole role,
        string passwordHash)
        : base(id)
    {
        Email = email;
        FullName = fullName;
        Role = role;
        PasswordHash = passwordHash;
    }

    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public long PermissionVersion { get; private set; } = 1;
    public string? PasswordSetupTokenHash { get; private set; }
    public DateTimeOffset? PasswordSetupTokenExpiresAt { get; private set; }
    public IReadOnlyCollection<UserPermission> Permissions => _permissions.AsReadOnly();
    public bool IsPasswordSetupRequired => PasswordSetupTokenHash is not null;

    public static User CreateCustomer(
        string email,
        string fullName,
        string passwordHash,
        IEnumerable<PermissionCode>? permissions = null)
    {
        ValidateEmail(email);
        ValidateFullName(fullName);
        ValidatePasswordHash(passwordHash);

        var user = new User(Guid.NewGuid(), NormalizeEmail(email), fullName.Trim(), UserRole.Customer, passwordHash);
        user.SetPermissions(permissions ?? DefaultPermissions.ForCustomer(), incrementVersion: false);

        return user;
    }

    public static User CreateAdministrator(
        string email,
        string fullName,
        string passwordHash)
    {
        ValidateEmail(email);
        ValidateFullName(fullName);
        ValidatePasswordHash(passwordHash);

        var user = new User(Guid.NewGuid(), NormalizeEmail(email), fullName.Trim(), UserRole.Administrator, passwordHash);
        user.SetPermissions(DefaultPermissions.ForAdministrator(), incrementVersion: false);

        return user;
    }

    public void UpdateProfile(string fullName)
    {
        ValidateFullName(fullName);

        FullName = fullName.Trim();
    }

    public void ReplacePermissions(IEnumerable<PermissionCode> permissions)
    {
        SetPermissions(permissions, incrementVersion: true);
    }

    public bool HasPermission(PermissionCode permission)
    {
        return _permissions.Any(userPermission => userPermission.Permission == permission);
    }

    public void StartPasswordSetup(string setupTokenHash, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(setupTokenHash))
        {
            throw new DomainException("Password setup token hash cannot be empty.");
        }

        PasswordSetupTokenHash = setupTokenHash;
        PasswordSetupTokenExpiresAt = expiresAt;
    }

    public bool HasActivePasswordSetupToken(DateTimeOffset now)
    {
        return PasswordSetupTokenHash is not null
            && PasswordSetupTokenExpiresAt is not null
            && PasswordSetupTokenExpiresAt > now;
    }

    public void CompletePasswordSetup(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);

        PasswordHash = passwordHash;
        PasswordSetupTokenHash = null;
        PasswordSetupTokenExpiresAt = null;
    }

    private void SetPermissions(IEnumerable<PermissionCode> permissions, bool incrementVersion)
    {
        var distinctPermissions = permissions.Distinct().ToArray();
        DefaultPermissions.EnsureAssignable(Role, distinctPermissions);
        var requestedPermissions = distinctPermissions.ToHashSet();

        for (var index = _permissions.Count - 1; index >= 0; index--)
        {
            if (!requestedPermissions.Contains(_permissions[index].Permission))
            {
                _permissions.RemoveAt(index);
            }
        }

        var existingPermissions = _permissions
            .Select(userPermission => userPermission.Permission)
            .ToHashSet();

        foreach (var permission in distinctPermissions)
        {
            if (!existingPermissions.Contains(permission))
            {
                _permissions.Add(UserPermission.Create(Id, permission));
            }
        }

        if (incrementVersion)
        {
            PermissionVersion++;
        }
    }

    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email cannot be empty.");
        }

        if (NormalizeEmail(email).Length > MaxEmailLength)
        {
            throw new DomainException($"Email cannot exceed {MaxEmailLength} characters.");
        }
    }

    private static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Customer name cannot be empty.");
        }

        if (fullName.Trim().Length > MaxFullNameLength)
        {
            throw new DomainException($"Customer name cannot exceed {MaxFullNameLength} characters.");
        }
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("Password hash cannot be empty.");
        }
    }
}
