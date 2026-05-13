using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }

    private Product() { } // EF Core

    public static Product Create(string name, string description, decimal price, int stock)
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
            IsActive = true
        };
    }

    public void Update(string name, string description, decimal price, int stock)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        SetUpdatedAt();
    }

    public void Deactivate() 
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
