namespace WholesalePlatform.WebApi.Contracts.Products;

public sealed record UpdateProductRequest(
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    bool IsAvailable,
    long Version);
