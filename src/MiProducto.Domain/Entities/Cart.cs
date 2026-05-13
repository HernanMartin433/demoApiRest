using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; private set; }
    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    private Cart() { }

    public static Cart Create(Guid userId)
    {
        return new Cart { UserId = userId };
    }
}