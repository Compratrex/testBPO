using Microsoft.EntityFrameworkCore;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Orders;

namespace WholesalePlatform.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Order?> GetByIdForCustomerAsync(
        Guid orderId,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return _context.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(
                order => order.Id == orderId && order.CustomerId == customerId,
                cancellationToken);
    }

    public async Task<PagedResult<Order>> ListByCustomerIdAsync(
        Guid customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.CustomerId == customerId);

        var totalCount = await query.CountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Order>(orders, page, pageSize, totalCount);
    }

    public Task<bool> HasActiveOrdersAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return _context.Orders.AnyAsync(
            order => order.CustomerId == customerId && order.Status != OrderStatus.Cancelled,
            cancellationToken);
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        return _context.Orders.AddAsync(order, cancellationToken).AsTask();
    }
}
