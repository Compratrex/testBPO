namespace WholesalePlatform.Application.Common.Models;

public sealed record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    bool IsAvailable,
    long Version);
