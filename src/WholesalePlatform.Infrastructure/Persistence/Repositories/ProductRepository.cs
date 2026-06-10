using Microsoft.EntityFrameworkCore;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Product>> ListAsync(
        bool onlyAvailable,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(product => !onlyAvailable || product.IsAvailable);

        var totalCount = await query.CountAsync(cancellationToken);
        var products = await query
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(products, page, pageSize, totalCount);
    }

    public async Task<IReadOnlyList<Product>> ListAvailableByIdsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(product => productIds.Contains(product.Id) && product.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken)
    {
        var normalizedSku = Product.NormalizeSku(sku);

        return _context.Products.AnyAsync(product => product.Sku == normalizedSku, cancellationToken);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        return _context.Products.AddAsync(product, cancellationToken).AsTask();
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
    }
}
