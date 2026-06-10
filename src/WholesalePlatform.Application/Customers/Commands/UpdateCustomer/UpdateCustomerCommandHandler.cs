using MediatR;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Customers.GetAccountByIdAsync(
            request.CustomerId,
            cancellationToken,
            includeUserPermissions: true);

        if (account is null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }

        if (account.Customer.Version != request.Version)
        {
            throw new DomainException("Customer was modified by another request. Reload it and try again.");
        }

        account.Customer.UpdateProfile(request.FullName, request.LegalAddress);
        account.PrimaryUser.UpdateProfile(request.FullName);
        account.PrimaryUser.ReplacePermissions(request.Permissions);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
