using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Orders;

namespace WholesalePlatform.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task<Order?> GetByIdForCustomerAsync(Guid orderId, Guid customerId, CancellationToken cancellationToken);
    Task<PagedResult<Order>> ListByCustomerIdAsync(
        Guid customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task<bool> HasActiveOrdersAsync(Guid customerId, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
}
