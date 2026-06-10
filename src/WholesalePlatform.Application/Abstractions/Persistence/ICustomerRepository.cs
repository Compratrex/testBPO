using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool includeUsers = false);
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<CustomerAccount?> GetAccountByIdAsync(
        Guid customerId,
        CancellationToken cancellationToken,
        bool includeUserPermissions = false);
    Task<PagedResult<CustomerAccount>> ListAccountsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    void Remove(Customer customer);
}
