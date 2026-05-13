using Microsoft.EntityFrameworkCore;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Domain.Entities;

namespace MiProducto.Infrastructure.Persistence.Repositories;

public class SubrubroRepository : ISubrubroRepository
{
    private readonly AppDbContext _context;
    public SubrubroRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Subrubro>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Subrubros
            .Include(s => s.Rubro)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Subrubro>> GetByRubroIdAsync(Guid rubroId, CancellationToken cancellationToken = default)
        => await _context.Subrubros
            .Include(s => s.Rubro)
            .Where(s => s.RubroId == rubroId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public async Task<Subrubro?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Subrubros
            .Include(s => s.Rubro)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddAsync(Subrubro subrubro, CancellationToken cancellationToken = default)
        => await _context.Subrubros.AddAsync(subrubro, cancellationToken);

    public void Update(Subrubro subrubro) => _context.Subrubros.Update(subrubro);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Subrubros.AnyAsync(s => s.Id == id && s.IsActive, cancellationToken);
}