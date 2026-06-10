using MediatR;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Application.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _unitOfWork.Customers.ListAccountsAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = customers.Items
            .Select(account => new CustomerDto(
                account.Customer.Id,
                account.PrimaryUser.Email,
                account.PrimaryUser.FullName,
                account.Customer.LegalAddress,
                account.PrimaryUser.Permissions.Select(permission => permission.Permission).ToArray(),
                account.Customer.Version))
            .ToArray();

        return new PagedResult<CustomerDto>(items, customers.Page, customers.PageSize, customers.TotalCount);
    }
}
