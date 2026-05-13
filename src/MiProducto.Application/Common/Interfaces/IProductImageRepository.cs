using MiProducto.Domain.Entities;

namespace MiProducto.Application.Common.Interfaces;

public interface IProductImageRepository
{
    Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductImage image, CancellationToken cancellationToken = default);
    void Delete(ProductImage image);
    Task<int> CountByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}