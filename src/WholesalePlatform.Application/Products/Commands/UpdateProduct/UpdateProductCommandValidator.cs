using FluentValidation;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.ProductId).NotEmpty();
        RuleFor(command => command.Sku).NotEmpty().MaximumLength(Product.MaxSkuLength);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Product.MaxNameLength);
        RuleFor(command => command.Description).MaximumLength(Product.MaxDescriptionLength);
        RuleFor(command => command.Price).GreaterThan(0);
        RuleFor(command => command.Version).GreaterThan(0);
    }
}
