using MediatR;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid CustomerId,
    string FullName,
    string LegalAddress,
    IReadOnlyCollection<PermissionCode> Permissions,
    long Version) : IRequest;
