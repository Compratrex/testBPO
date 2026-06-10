using MediatR;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Application.Customers.Commands.RegisterCustomer;

public sealed record RegisterCustomerCommand(
    string Email,
    string FullName,
    string LegalAddress,
    IReadOnlyCollection<PermissionCode> Permissions) : IRequest<CreatedIdDto>;

