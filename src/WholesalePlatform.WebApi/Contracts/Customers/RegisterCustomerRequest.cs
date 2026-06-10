using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.WebApi.Contracts.Customers;

public sealed record RegisterCustomerRequest(
    string Email,
    string FullName,
    string LegalAddress,
    IReadOnlyCollection<PermissionCode> Permissions);

