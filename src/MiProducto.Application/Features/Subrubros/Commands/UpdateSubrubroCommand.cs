using FluentValidation;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Subrubros.Commands;

public record UpdateSubrubroCommand(Guid Id, string Name, Guid RubroId) : IRequest<SubrubroDto?>;

public class UpdateSubrubroCommandValidator : AbstractValidator<UpdateSubrubroCommand>
{
    public UpdateSubrubroCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido.").MaximumLength(200);
        RuleFor(x => x.RubroId).NotEmpty().WithMessage("El rubro es requerido.");
    }
}

public class UpdateSubrubroCommandHandler : IRequestHandler<UpdateSubrubroCommand, SubrubroDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateSubrubroCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<SubrubroDto?> Handle(UpdateSubrubroCommand request, CancellationToken cancellationToken)
    {
        var subrubro = await _unitOfWork.Subrubros.GetByIdAsync(request.Id, cancellationToken);
        if (subrubro is null) return null;
        subrubro.Update(request.Name, request.RubroId);
        _unitOfWork.Subrubros.Update(subrubro);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var rubro = await _unitOfWork.Rubros.GetByIdAsync(request.RubroId, cancellationToken);
        return new SubrubroDto(subrubro.Id, subrubro.Name, subrubro.IsActive, subrubro.RubroId, rubro?.Name ?? "");
    }
}