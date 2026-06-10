namespace WholesalePlatform.WebApi.Contracts.Products;

public sealed record CreateProductRequest(
    string Sku,
    string Name,
    string? Description,
    decimal Price);

