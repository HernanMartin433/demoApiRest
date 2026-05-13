using MiProducto.Domain.Entities;

namespace MiProducto.Application.Common.Interfaces;

public interface ISubrubroRepository
{
    Task<IEnumerable<Subrubro>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Subrubro>> GetByRubroIdAsync(Guid rubroId, CancellationToken cancellationToken = default);
    Task<Subrubro?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Subrubro subrubro, CancellationToken cancellationToken = default);
    void Update(Subrubro subrubro);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}