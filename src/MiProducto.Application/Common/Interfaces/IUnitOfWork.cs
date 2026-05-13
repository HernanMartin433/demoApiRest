namespace MiProducto.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IProductImageRepository ProductImages { get; }
    IRubroRepository Rubros { get; }
    ISubrubroRepository Subrubros { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}