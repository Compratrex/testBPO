using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Product>> ListAsync(
        bool onlyAvailable,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> ListAvailableByIdsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken);
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
    void Remove(Product product);
}
