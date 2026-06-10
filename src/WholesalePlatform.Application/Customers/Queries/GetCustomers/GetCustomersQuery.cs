using MediatR;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery(
    int Page = 1,
    int PageSize = 50) : IRequest<PagedResult<CustomerDto>>;
