using Microsoft.EntityFrameworkCore;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Domain.Entities;

namespace MiProducto.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim(), cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(user, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users.AnyAsync(u => u.Email == email.ToLower().Trim(), cancellationToken);

    public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken); 
}