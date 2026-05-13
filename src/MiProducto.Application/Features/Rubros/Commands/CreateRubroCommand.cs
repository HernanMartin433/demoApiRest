using FluentValidation;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Rubros.Commands;

public record CreateRubroCommand(string Name) : IRequest<RubroDto>;

public class CreateRubroCommandValidator : AbstractValidator<CreateRubroCommand>
{
    public CreateRubroCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido.").MaximumLength(200);
    }
}

public class CreateRubroCommandHandler : IRequestHandler<CreateRubroCommand, RubroDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateRubroCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<RubroDto> Handle(CreateRubroCommand request, CancellationToken cancellationToken)
    {
        var rubro = Rubro.Create(request.Name);
        await _unitOfWork.Rubros.AddAsync(rubro, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RubroDto(rubro.Id, rubro.Name, rubro.IsActive, Enumerable.Empty<SubrubroDto>());
    }
}