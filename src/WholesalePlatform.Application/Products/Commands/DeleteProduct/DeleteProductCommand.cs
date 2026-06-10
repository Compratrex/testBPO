using MediatR;

namespace WholesalePlatform.Application.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid ProductId) : IRequest;

