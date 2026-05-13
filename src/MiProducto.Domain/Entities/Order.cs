using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public enum OrderStatus { Pending, Confirmed, Cancelled }
public enum PaymentStatus { Pending, Approved, Rejected }
public enum PaymentMethod { Debit, CreditVisa, MercadoPago }

public class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order() { }

    public static Order Create(Guid userId, IEnumerable<CartItem> cartItems)
    {
        var items = cartItems.ToList();
        var total = items.Sum(i => i.UnitPrice * i.Quantity);

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            Total = total
        };

        foreach (var item in items)
            order.Items.Add(OrderItem.Create(order.Id, item.ProductId, item.UnitPrice, item.Quantity, item.Product?.Name ?? ""));

        return order;
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        SetUpdatedAt();
    }

    public void ProcessPayment(PaymentMethod method, bool approved)
    {
        PaymentMethod = method;
        PaymentStatus = approved ? PaymentStatus.Approved : PaymentStatus.Rejected;
        if (approved) PaidAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
        SetUpdatedAt();
    }
}