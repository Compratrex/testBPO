using FluentValidation;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Users;
using WholesalePlatform.Domain.Permissions;

namespace WholesalePlatform.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(command => command.CustomerId).NotEmpty();
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(User.MaxFullNameLength);
        RuleFor(command => command.LegalAddress).NotEmpty().MaximumLength(Customer.MaxLegalAddressLength);
        RuleForEach(command => command.Permissions).IsInEnum();
        RuleForEach(command => command.Permissions)
            .Must(DefaultPermissions.IsAllowedForCustomer)
            .WithMessage("Customer cannot be assigned management permissions.");
        RuleFor(command => command.Version).GreaterThan(0);
    }
}
