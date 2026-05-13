using FluentValidation;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid RubroId,
    Guid SubrubroId
) : IRequest<ProductDto>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(1000);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
        RuleFor(x => x.RubroId).NotEmpty().WithMessage("El rubro es requerido.");
        RuleFor(x => x.SubrubroId).NotEmpty().WithMessage("El subrubro es requerido.");
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.RubroId, 
            request.SubrubroId);

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

       return new ProductDto(
            product.Id, product.Name, product.Description,
            product.Price, product.Stock, product.IsActive,
            product.RubroId, "",
            product.SubrubroId, "",
            product.Images.OrderBy(i => i.Order).Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.Order)),
            product.CreatedAt, product.UpdatedAt);
    }
}