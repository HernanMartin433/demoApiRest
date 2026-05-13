using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = default!;
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    public static OrderItem Create(Guid orderId, Guid productId, decimal unitPrice, int quantity, string productName)
    {
        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            UnitPrice = unitPrice,
            Quantity = quantity,
            ProductName = productName
        };
    }
}