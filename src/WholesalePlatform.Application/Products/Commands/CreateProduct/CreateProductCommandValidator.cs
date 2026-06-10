using FluentValidation;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Sku).NotEmpty().MaximumLength(Product.MaxSkuLength);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Product.MaxNameLength);
        RuleFor(command => command.Description).MaximumLength(Product.MaxDescriptionLength);
        RuleFor(command => command.Price).GreaterThan(0);
    }
}
