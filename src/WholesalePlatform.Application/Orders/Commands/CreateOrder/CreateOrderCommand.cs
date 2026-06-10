using MediatR;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    IReadOnlyCollection<CreateOrderItemCommand> Items) : IRequest<CreatedIdDto>;

public sealed record CreateOrderItemCommand(Guid ProductId, int Quantity);

