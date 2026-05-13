using MiProducto.Application.Common.Interfaces;
using MiProducto.Infrastructure.Persistence.Repositories;

namespace MiProducto.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ProductRepository? _products;
    private UserRepository? _users;
    private RefreshTokenRepository? _refreshTokens;
    private ProductImageRepository? _productImages;
    private RubroRepository? _rubros;
    private SubrubroRepository? _subrubros;
    private CartRepository? _carts;
    private OrderRepository? _orders;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
    public IProductImageRepository ProductImages => _productImages ??= new ProductImageRepository(_context);
    public IRubroRepository Rubros => _rubros ??= new RubroRepository(_context);
    public ISubrubroRepository Subrubros => _subrubros ??= new SubrubroRepository(_context);
    public ICartRepository Carts => _carts ??= new CartRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}