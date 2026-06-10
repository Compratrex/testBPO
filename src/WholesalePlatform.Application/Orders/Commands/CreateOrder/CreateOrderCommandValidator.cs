using FluentValidation;

namespace WholesalePlatform.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.Items).NotEmpty();
        RuleForEach(command => command.Items).ChildRules(item =>
        {
            item.RuleFor(command => command.ProductId).NotEmpty();
            item.RuleFor(command => command.Quantity).GreaterThan(0);
        });
    }
}

