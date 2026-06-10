using System.Data;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Orders;
using WholesalePlatform.Domain.Products;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Tests.TestDoubles;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public FakeUnitOfWork(
        IEnumerable<User>? users = null,
        IEnumerable<Customer>? customers = null,
        IEnumerable<Product>? products = null,
        IEnumerable<Order>? orders = null)
    {
        Users = new FakeUserRepository(users);
        Customers = new FakeCustomerRepository(customers, Users);
        Products = new FakeProductRepository(products);
        Orders = new FakeOrderRepository(orders);
    }

    public IUserRepository Users { get; }
    public ICustomerRepository Customers { get; }
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public int SaveChangesCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }

    public Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
    {
        return action(cancellationToken);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeUserRepository(IEnumerable<User>? users)
        {
            _users = users?.ToList() ?? [];
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool includePermissions = false)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Id == id));
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken, bool includePermissions = false)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.Any(user => user.Email == email));
        }

        public Task AddAsync(User user, CancellationToken cancellationToken)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public void Remove(User user)
        {
            _users.Remove(user);
        }

        public User? Find(Guid id)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers;
        private readonly FakeUserRepository _users;

        public FakeCustomerRepository(IEnumerable<Customer>? customers, IUserRepository users)
        {
            _customers = customers?.ToList() ?? [];
            _users = (FakeUserRepository)users;
        }

        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool includeUsers = false)
        {
            return Task.FromResult(_customers.FirstOrDefault(customer => customer.Id == id));
        }

        public Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_customers.FirstOrDefault(
                customer => customer.Users.Any(customerUser => customerUser.UserId == userId)));
        }

        public Task<CustomerAccount?> GetAccountByIdAsync(
            Guid customerId,
            CancellationToken cancellationToken,
            bool includeUserPermissions = false)
        {
            var customer = _customers.FirstOrDefault(customer => customer.Id == customerId);
            var primaryUserId = customer?.Users.FirstOrDefault(user => user.IsPrimaryContact)?.UserId;
            var primaryUser = primaryUserId is null ? null : _users.Find(primaryUserId.Value);

            return Task.FromResult(
                customer is null || primaryUser is null
                    ? null
                    : new CustomerAccount(customer, primaryUser));
        }

        public Task<PagedResult<CustomerAccount>> ListAccountsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var accounts = _customers
                .Select(customer =>
                {
                    var primaryUserId = customer.Users.FirstOrDefault(user => user.IsPrimaryContact)?.UserId;
                    var primaryUser = primaryUserId is null ? null : _users.Find(primaryUserId.Value);

                    return primaryUser is null ? null : new CustomerAccount(customer, primaryUser);
                })
                .OfType<CustomerAccount>()
                .ToList();

            var items = accounts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<CustomerAccount>(items, page, pageSize, accounts.Count));
        }

        public Task AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            _customers.Add(customer);
            return Task.CompletedTask;
        }

        public void Remove(Customer customer)
        {
            _customers.Remove(customer);
        }
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public FakeProductRepository(IEnumerable<Product>? products)
        {
            _products = products?.ToList() ?? [];
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_products.FirstOrDefault(product => product.Id == id));
        }

        public Task<PagedResult<Product>> ListAsync(
            bool onlyAvailable,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var products = _products
                .Where(product => !onlyAvailable || product.IsAvailable)
                .ToList();
            var items = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Product>(items, page, pageSize, products.Count));
        }

        public Task<IReadOnlyList<Product>> ListAvailableByIdsAsync(
            IReadOnlyCollection<Guid> productIds,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Product>>(
                _products.Where(product => productIds.Contains(product.Id) && product.IsAvailable).ToList());
        }

        public Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken)
        {
            return Task.FromResult(_products.Any(product => product.Sku == sku));
        }

        public Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public void Remove(Product product)
        {
            _products.Remove(product);
        }
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders;

        public FakeOrderRepository(IEnumerable<Order>? orders)
        {
            _orders = orders?.ToList() ?? [];
        }

        public Task<Order?> GetByIdForCustomerAsync(
            Guid orderId,
            Guid customerId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_orders.FirstOrDefault(
                order => order.Id == orderId && order.CustomerId == customerId));
        }

        public Task<PagedResult<Order>> ListByCustomerIdAsync(
            Guid customerId,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var orders = _orders.Where(order => order.CustomerId == customerId).ToList();
            var items = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Order>(items, page, pageSize, orders.Count));
        }

        public Task<bool> HasActiveOrdersAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_orders.Any(
                order => order.CustomerId == customerId && order.Status != OrderStatus.Cancelled));
        }

        public Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            _orders.Add(order);
            return Task.CompletedTask;
        }
    }
}
