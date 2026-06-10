using System.Data;
using Microsoft.EntityFrameworkCore;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Domain.Orders;
using WholesalePlatform.Domain.Products;
using WholesalePlatform.Domain.Users;
using WholesalePlatform.Infrastructure.Persistence.Repositories;

namespace WholesalePlatform.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Customers = new CustomerRepository(context);
        Products = new ProductRepository(context);
        Orders = new OrderRepository(context);
    }

    public IUserRepository Users { get; }
    public ICustomerRepository Customers { get; }
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(
                isolationLevel,
                cancellationToken);

            await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}
