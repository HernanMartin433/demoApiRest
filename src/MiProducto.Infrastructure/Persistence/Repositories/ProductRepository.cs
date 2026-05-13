using Microsoft.EntityFrameworkCore;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Domain.Entities;

namespace MiProducto.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products.FindAsync([id], cancellationToken);

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

    
    /*public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
    int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
        .Include(p => p.Images)
        .Include(p => p.Rubro)
        .Include(p => p.Subrubro)
        .Where(p => p.IsActive)
        .OrderBy(p => p.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

        return (items, totalCount);
    }*/
    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize,
        string? search = null,
        Guid? rubroId = null,
        Guid? subrubroId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Include(p => p.Images)
            .Include(p => p.Rubro)
            .Include(p => p.Subrubro)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

        if (rubroId.HasValue)
            query = query.Where(p => p.RubroId == rubroId.Value);

        if (subrubroId.HasValue)
            query = query.Where(p => p.SubrubroId == subrubroId.Value);

        query = query.OrderBy(p => p.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
    public async Task<Product?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products
            .Include(p => p.Images)
            .Include(p => p.Rubro)
            .Include(p => p.Subrubro)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await _context.Products.AddAsync(product, cancellationToken);

    public void Update(Product product)
        => _context.Products.Update(product);

    public void Delete(Product product)
        => _context.Products.Remove(product);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
}