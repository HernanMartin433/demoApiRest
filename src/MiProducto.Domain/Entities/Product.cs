using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public Guid RubroId { get; private set; }
    public Rubro Rubro { get; private set; } = default!;
    public Guid SubrubroId { get; private set; }
    public Subrubro Subrubro { get; private set; } = default!;
    public ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();

    private Product() { }

    public static Product Create(string name, string description, decimal price, int stock, Guid rubroId, Guid subrubroId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stock);

        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            IsActive = true,
            RubroId = rubroId,
            SubrubroId = subrubroId
        };
    }

    public void Update(string name, string description, decimal price, int stock, Guid rubroId, Guid subrubroId)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        RubroId = rubroId;
        SubrubroId = subrubroId;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}