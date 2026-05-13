using Microsoft.EntityFrameworkCore;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Domain.Entities;

namespace MiProducto.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    public CartRepository(AppDbContext context) => _context = context;

    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
        => await _context.Carts.AddAsync(cart, cancellationToken);

    public void Update(Cart cart) => _context.Carts.Update(cart);
    public async Task AddItemAsync(CartItem item, CancellationToken cancellationToken = default)
    => await _context.CartItems.AddAsync(item, cancellationToken);
}