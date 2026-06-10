using System.Data;
using MediatR;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Exceptions;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Orders;

namespace WholesalePlatform.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreatedIdDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CreatedIdDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        Order? order = null;

        await _unitOfWork.ExecuteInTransactionAsync(
            async transactionCancellationToken =>
            {
                var customer = await _unitOfWork.Customers.GetByUserIdAsync(
                    userId,
                    transactionCancellationToken);

                if (customer is null)
                {
                    throw new ForbiddenAccessException("Only active customers can create orders.");
                }

                var productIds = request.Items.Select(item => item.ProductId).Distinct().ToArray();
                var products = await _unitOfWork.Products.ListAvailableByIdsAsync(
                    productIds,
                    transactionCancellationToken);

                var productsById = products.ToDictionary(product => product.Id);
                order = Order.Create(customer.Id, userId);

                foreach (var item in request.Items)
                {
                    if (!productsById.TryGetValue(item.ProductId, out var product))
                    {
                        throw new KeyNotFoundException($"Product '{item.ProductId}' not found or unavailable.");
                    }

                    order.AddItem(product.Id, product.Sku, product.Name, product.Price, item.Quantity);
                }

                await _unitOfWork.Orders.AddAsync(order, transactionCancellationToken);
                await _unitOfWork.SaveChangesAsync(transactionCancellationToken);
            },
            cancellationToken,
            IsolationLevel.Serializable);

        return new CreatedIdDto(order!.Id);
    }
}
