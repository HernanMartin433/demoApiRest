using MediatR;
using MiProducto.Application.Common.Interfaces;

namespace MiProducto.Application.Features.Rubros.Commands;

public record DeleteRubroCommand(Guid Id) : IRequest<bool>;

public class DeleteRubroCommandHandler : IRequestHandler<DeleteRubroCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteRubroCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteRubroCommand request, CancellationToken cancellationToken)
    {
        var rubro = await _unitOfWork.Rubros.GetByIdAsync(request.Id, cancellationToken);
        if (rubro is null) return false;
        rubro.Deactivate();
        _unitOfWork.Rubros.Update(rubro);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}