using FluentValidation;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Permissions;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Application.Customers.Commands.RegisterCustomer;

public sealed class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress().MaximumLength(User.MaxEmailLength);
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(User.MaxFullNameLength);
        RuleFor(command => command.LegalAddress).NotEmpty().MaximumLength(Customer.MaxLegalAddressLength);
        RuleForEach(command => command.Permissions).IsInEnum();
        RuleForEach(command => command.Permissions)
            .Must(DefaultPermissions.IsAllowedForCustomer)
            .WithMessage("Customer cannot be assigned management permissions.");
    }
}
