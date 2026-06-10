using Microsoft.EntityFrameworkCore;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Customer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        bool includeUsers = false)
    {
        return ApplyUsers(_context.Customers, includeUsers)
            .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
    }

    public Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _context.Customers
            .Include(customer => customer.Users)
            .FirstOrDefaultAsync(
                customer => customer.Users.Any(customerUser => customerUser.UserId == userId),
                cancellationToken);
    }

    public async Task<CustomerAccount?> GetAccountByIdAsync(
        Guid customerId,
        CancellationToken cancellationToken,
        bool includeUserPermissions = false)
    {
        var customer = await _context.Customers
            .Include(customer => customer.Users)
            .FirstOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);

        if (customer is null)
        {
            return null;
        }

        var primaryUserId = customer.Users
            .FirstOrDefault(customerUser => customerUser.IsPrimaryContact)
            ?.UserId;

        if (primaryUserId is null)
        {
            return null;
        }

        var userQuery = _context.Users.AsQueryable();
        if (includeUserPermissions)
        {
            userQuery = userQuery.Include(user => user.Permissions);
        }

        var primaryUser = await userQuery.FirstOrDefaultAsync(
            user => user.Id == primaryUserId.Value,
            cancellationToken);

        return primaryUser is null ? null : new CustomerAccount(customer, primaryUser);
    }

    public async Task<PagedResult<CustomerAccount>> ListAccountsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Customers
            .AsNoTracking()
            .Include(customer => customer.Users);

        var totalCount = await query.CountAsync(cancellationToken);
        var customers = await query
            .OrderBy(customer => customer.Name)
            .ThenBy(customer => customer.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var primaryUserIds = customers
            .Select(customer => customer.Users.FirstOrDefault(user => user.IsPrimaryContact)?.UserId)
            .OfType<Guid>()
            .ToArray();

        var primaryUsers = await _context.Users
            .AsNoTracking()
            .Include(user => user.Permissions)
            .Where(user => primaryUserIds.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, cancellationToken);

        var accounts = customers
            .Select(customer =>
            {
                var primaryUserId = customer.Users.FirstOrDefault(user => user.IsPrimaryContact)?.UserId;
                return primaryUserId is not null && primaryUsers.TryGetValue(primaryUserId.Value, out var user)
                    ? new CustomerAccount(customer, user)
                    : null;
            })
            .OfType<CustomerAccount>()
            .ToList();

        return new PagedResult<CustomerAccount>(accounts, page, pageSize, totalCount);
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        return _context.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public void Remove(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    private static IQueryable<Customer> ApplyUsers(IQueryable<Customer> query, bool includeUsers)
    {
        return includeUsers ? query.Include(customer => customer.Users) : query;
    }
}
