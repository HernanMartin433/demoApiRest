using FluentValidation;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Rubros.Commands;

public record UpdateRubroCommand(Guid Id, string Name) : IRequest<RubroDto?>;

public class UpdateRubroCommandValidator : AbstractValidator<UpdateRubroCommand>
{
    public UpdateRubroCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido.").MaximumLength(200);
    }
}

public class UpdateRubroCommandHandler : IRequestHandler<UpdateRubroCommand, RubroDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateRubroCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<RubroDto?> Handle(UpdateRubroCommand request, CancellationToken cancellationToken)
    {
        var rubro = await _unitOfWork.Rubros.GetByIdWithSubrubrosAsync(request.Id, cancellationToken);
        if (rubro is null) return null;
        rubro.Update(request.Name);
        _unitOfWork.Rubros.Update(rubro);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RubroDto(rubro.Id, rubro.Name, rubro.IsActive,
            rubro.Subrubros.Select(s => new SubrubroDto(s.Id, s.Name, s.IsActive, s.RubroId, rubro.Name)));
    }
}