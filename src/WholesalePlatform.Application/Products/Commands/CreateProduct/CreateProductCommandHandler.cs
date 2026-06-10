using MediatR;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreatedIdDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatedIdDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Products.SkuExistsAsync(request.Sku, cancellationToken))
        {
            throw new InvalidOperationException("Product with the same SKU already exists.");
        }

        var product = Product.Create(request.Sku, request.Name, request.Description, request.Price);

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatedIdDto(product.Id);
    }
}
