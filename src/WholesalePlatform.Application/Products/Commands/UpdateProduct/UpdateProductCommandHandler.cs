using MediatR;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        if (product.Version != request.Version)
        {
            throw new DomainException("Product was modified by another request. Reload it and try again.");
        }

        product.Update(request.Sku, request.Name, request.Description, request.Price, request.IsAvailable);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
