using MediatR;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Email;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Permissions;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Application.Customers.Commands.RegisterCustomer;

public sealed class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, CreatedIdDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailOutbox _emailOutbox;

    public RegisterCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordGenerator passwordGenerator,
        IPasswordHasher passwordHasher,
        IEmailOutbox emailOutbox)
    {
        _unitOfWork = unitOfWork;
        _passwordGenerator = passwordGenerator;
        _passwordHasher = passwordHasher;
        _emailOutbox = emailOutbox;
    }

    public async Task<CreatedIdDto> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("User with the same email already exists.");
        }

        var temporaryPassword = _passwordGenerator.Generate();
        var permissions = request.Permissions.Count > 0
            ? request.Permissions
            : DefaultPermissions.ForCustomer();

        var primaryUser = User.CreateCustomer(
            request.Email,
            request.FullName,
            _passwordHasher.HashPassword(temporaryPassword),
            permissions);
        var customer = Customer.Create(request.FullName, request.LegalAddress, primaryUser.Id);

        await _unitOfWork.ExecuteInTransactionAsync(
            async transactionCancellationToken =>
            {
                await _unitOfWork.Users.AddAsync(primaryUser, transactionCancellationToken);
                await _unitOfWork.Customers.AddAsync(customer, transactionCancellationToken);
                await _emailOutbox.EnqueueAsync(
                    primaryUser.Email,
                    "Wholesale platform login details",
                    $"Login: {primaryUser.Email}\nTemporary password: {temporaryPassword}\nPlease change this password after first login.",
                    transactionCancellationToken);

                await _unitOfWork.SaveChangesAsync(transactionCancellationToken);
            },
            cancellationToken);

        return new CreatedIdDto(customer.Id);
    }
}
