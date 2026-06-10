using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Domain.Orders;

public sealed class OrderItem : AuditableEntity
{
    private OrderItem()
    {
    }

    private OrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        string productSku,
        string productName,
        decimal unitPrice,
        int quantity)
        : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductSku = productSku;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        LineTotal = unitPrice * quantity;
    }

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductSku { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal { get; private set; }

    internal static OrderItem Create(
        Guid orderId,
        Guid productId,
        string productSku,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("Order item product id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(productSku))
        {
            throw new DomainException("Order item product SKU cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException("Order item product name cannot be empty.");
        }

        if (unitPrice <= 0)
        {
            throw new DomainException("Order item unit price must be greater than zero.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Order item quantity must be greater than zero.");
        }

        return new OrderItem(Guid.NewGuid(), orderId, productId, productSku, productName, unitPrice, quantity);
    }
}
