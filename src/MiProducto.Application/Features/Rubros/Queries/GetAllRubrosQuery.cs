using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Rubros.Queries;

public record GetAllRubrosQuery : IRequest<IEnumerable<RubroDto>>;

public class GetAllRubrosQueryHandler : IRequestHandler<GetAllRubrosQuery, IEnumerable<RubroDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllRubrosQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<RubroDto>> Handle(GetAllRubrosQuery request, CancellationToken cancellationToken)
    {
        var rubros = await _unitOfWork.Rubros.GetAllAsync(cancellationToken);
        return rubros.Select(r => new RubroDto(
            r.Id, r.Name, r.IsActive,
            r.Subrubros.Select(s => new SubrubroDto(s.Id, s.Name, s.IsActive, s.RubroId, r.Name))));
    }
}