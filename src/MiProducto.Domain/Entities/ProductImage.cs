using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public string ImageUrl { get; private set; } = default!;
    public int Order { get; private set; }

    private ProductImage() { }

    public static ProductImage Create(Guid productId, string imageUrl, int order)
    {
        return new ProductImage
        {
            ProductId = productId,
            ImageUrl = imageUrl,
            Order = order
        };
    }
}