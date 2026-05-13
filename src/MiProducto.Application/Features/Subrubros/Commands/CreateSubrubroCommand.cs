using FluentValidation;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Subrubros.Commands;

public record CreateSubrubroCommand(string Name, Guid RubroId) : IRequest<SubrubroDto>;

public class CreateSubrubroCommandValidator : AbstractValidator<CreateSubrubroCommand>
{
    public CreateSubrubroCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido.").MaximumLength(200);
        RuleFor(x => x.RubroId).NotEmpty().WithMessage("El rubro es requerido.");
    }
}

public class CreateSubrubroCommandHandler : IRequestHandler<CreateSubrubroCommand, SubrubroDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateSubrubroCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<SubrubroDto> Handle(CreateSubrubroCommand request, CancellationToken cancellationToken)
    {
        var rubroExists = await _unitOfWork.Rubros.ExistsAsync(request.RubroId, cancellationToken);
        if (!rubroExists) throw new KeyNotFoundException("El rubro no existe.");

        var subrubro = Subrubro.Create(request.Name, request.RubroId);
        await _unitOfWork.Subrubros.AddAsync(subrubro, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var rubro = await _unitOfWork.Rubros.GetByIdAsync(request.RubroId, cancellationToken);
        return new SubrubroDto(subrubro.Id, subrubro.Name, subrubro.IsActive, subrubro.RubroId, rubro?.Name ?? "");
    }
}