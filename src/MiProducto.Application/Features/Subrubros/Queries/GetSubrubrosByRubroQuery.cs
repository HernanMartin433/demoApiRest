using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Subrubros.Queries;

public record GetSubrubrosByRubroQuery(Guid RubroId) : IRequest<IEnumerable<SubrubroDto>>;

public class GetSubrubrosByRubroQueryHandler : IRequestHandler<GetSubrubrosByRubroQuery, IEnumerable<SubrubroDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetSubrubrosByRubroQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<SubrubroDto>> Handle(GetSubrubrosByRubroQuery request, CancellationToken cancellationToken)
    {
        var subrubros = await _unitOfWork.Subrubros.GetByRubroIdAsync(request.RubroId, cancellationToken);
        return subrubros.Select(s => new SubrubroDto(s.Id, s.Name, s.IsActive, s.RubroId, s.Rubro?.Name ?? ""));
    }
}