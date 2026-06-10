using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Application.Common.Models;

public sealed record OrderDto(
    Guid Id,
    OrderStatus Status,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items,
    long Version);

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
