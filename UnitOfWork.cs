using MiProducto.Application.Common.Interfaces;
using MiProducto.Infrastructure.Persistence.Repositories;

namespace MiProducto.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ProductRepository? _products;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IProductRepository Products
        => _products ??= new ProductRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
