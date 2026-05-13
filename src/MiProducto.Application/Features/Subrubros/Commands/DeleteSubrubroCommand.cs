using MediatR;
using MiProducto.Application.Common.Interfaces;

namespace MiProducto.Application.Features.Subrubros.Commands;

public record DeleteSubrubroCommand(Guid Id) : IRequest<bool>;

public class DeleteSubrubroCommandHandler : IRequestHandler<DeleteSubrubroCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteSubrubroCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteSubrubroCommand request, CancellationToken cancellationToken)
    {
        var subrubro = await _unitOfWork.Subrubros.GetByIdAsync(request.Id, cancellationToken);
        if (subrubro is null) return false;
        subrubro.Deactivate();
        _unitOfWork.Subrubros.Update(subrubro);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}