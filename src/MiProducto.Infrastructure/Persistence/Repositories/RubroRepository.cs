using Microsoft.EntityFrameworkCore;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Domain.Entities;

namespace MiProducto.Infrastructure.Persistence.Repositories;

public class RubroRepository : IRubroRepository
{
    private readonly AppDbContext _context;
    public RubroRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Rubro>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Rubros
            .Include(r => r.Subrubros.Where(s => s.IsActive))
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

    public async Task<Rubro?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Rubros.FindAsync([id], cancellationToken);

    public async Task<Rubro?> GetByIdWithSubrubrosAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Rubros
            .Include(r => r.Subrubros)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task AddAsync(Rubro rubro, CancellationToken cancellationToken = default)
        => await _context.Rubros.AddAsync(rubro, cancellationToken);

    public void Update(Rubro rubro) => _context.Rubros.Update(rubro);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Rubros.AnyAsync(r => r.Id == id && r.IsActive, cancellationToken);
}