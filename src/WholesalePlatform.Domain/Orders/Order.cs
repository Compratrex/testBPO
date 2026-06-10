using WholesalePlatform.Domain.Common;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Domain.Orders;

public sealed class Order : AuditableEntity
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    private Order(Guid id, Guid customerId, Guid createdByUserId)
        : base(id)
    {
        CustomerId = customerId;
        CreatedByUserId = createdByUserId;
        Status = OrderStatus.PendingPayment;
        Currency = "USD";
    }

    public Guid CustomerId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(Guid customerId, Guid createdByUserId)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer id cannot be empty.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Order creator id cannot be empty.");
        }

        return new Order(Guid.NewGuid(), customerId, createdByUserId);
    }

    public void AddItem(Guid productId, string productSku, string productName, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new DomainException("Cannot edit an order after payment status changed.");
        }

        if (_items.Any(item => item.ProductId == productId))
        {
            throw new DomainException("Product is already added to the order.");
        }

        _items.Add(OrderItem.Create(Id, productId, productSku, productName, unitPrice, quantity));
        TotalAmount = _items.Sum(item => item.LineTotal);
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Paid)
        {
            throw new DomainException("Paid order cannot be cancelled.");
        }

        if (Status == OrderStatus.Cancelled)
        {
            return;
        }

        Status = OrderStatus.Cancelled;
    }
}
