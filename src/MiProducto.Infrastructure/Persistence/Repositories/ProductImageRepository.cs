using Microsoft.EntityFrameworkCore;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Domain.Entities;

namespace MiProducto.Infrastructure.Persistence.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly AppDbContext _context;

    public ProductImageRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _context.ProductImages
            .Where(i => i.ProductId == productId)
            .OrderBy(i => i.Order)
            .ToListAsync(cancellationToken);

    public async Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ProductImages.FindAsync([id], cancellationToken);

    public async Task AddAsync(ProductImage image, CancellationToken cancellationToken = default)
        => await _context.ProductImages.AddAsync(image, cancellationToken);

    public void Delete(ProductImage image)
        => _context.ProductImages.Remove(image);

    public async Task<int> CountByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _context.ProductImages.CountAsync(i => i.ProductId == productId, cancellationToken);
}