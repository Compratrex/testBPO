using MediatR;

namespace WholesalePlatform.Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    bool IsAvailable,
    long Version) : IRequest;
