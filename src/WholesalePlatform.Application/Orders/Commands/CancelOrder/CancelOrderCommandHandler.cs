using MediatR;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Persistence;

namespace WholesalePlatform.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var customer = await _unitOfWork.Customers.GetByUserIdAsync(userId, cancellationToken);
        if (customer is null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        var order = await _unitOfWork.Orders.GetByIdForCustomerAsync(
            request.OrderId,
            customer.Id,
            cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        order.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
