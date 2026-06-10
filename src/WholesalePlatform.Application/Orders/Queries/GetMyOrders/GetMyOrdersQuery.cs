using MediatR;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(
    int Page = 1,
    int PageSize = 50) : IRequest<PagedResult<OrderDto>>;
