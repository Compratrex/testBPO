using MediatR;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    decimal Price) : IRequest<CreatedIdDto>;

