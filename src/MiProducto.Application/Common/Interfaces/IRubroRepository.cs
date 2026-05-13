using MiProducto.Domain.Entities;

namespace MiProducto.Application.Common.Interfaces;

public interface IRubroRepository
{
    Task<IEnumerable<Rubro>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Rubro?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Rubro?> GetByIdWithSubrubrosAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Rubro rubro, CancellationToken cancellationToken = default);
    void Update(Rubro rubro);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}