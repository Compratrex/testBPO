using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Application.Common.Models;

public sealed record CustomerDto(
    Guid Id,
    string Email,
    string FullName,
    string LegalAddress,
    IReadOnlyCollection<PermissionCode> Permissions,
    long Version);
