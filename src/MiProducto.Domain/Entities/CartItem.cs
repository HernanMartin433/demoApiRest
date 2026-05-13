using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Cart Cart { get; private set; } = default!;
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private CartItem() { }

    public static CartItem Create(Guid cartId, Guid productId, decimal unitPrice, int quantity = 1)
    {
        return new CartItem
        {
            CartId = cartId,
            ProductId = productId,
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        Quantity = quantity;
        SetUpdatedAt();
    }
}