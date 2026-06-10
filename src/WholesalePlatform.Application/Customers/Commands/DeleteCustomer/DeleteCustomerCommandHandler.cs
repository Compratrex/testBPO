using MediatR;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Application.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Customers.GetAccountByIdAsync(
            request.CustomerId,
            cancellationToken,
            includeUserPermissions: true);

        if (account is null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }

        var hasActiveOrders = await _unitOfWork.Orders.HasActiveOrdersAsync(
            request.CustomerId,
            cancellationToken);

        if (hasActiveOrders)
        {
            throw new DomainException("Customer with active orders cannot be deleted.");
        }

        account.PrimaryUser.ReplacePermissions([]);
        _unitOfWork.Customers.Remove(account.Customer);
        _unitOfWork.Users.Remove(account.PrimaryUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
