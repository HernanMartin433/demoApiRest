using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return null;

        return new ProductDto(
            product.Id, product.Name, product.Description,
            product.Price, product.Stock, product.IsActive,
            product.RubroId, product.Rubro?.Name ?? "",
            product.SubrubroId, product.Subrubro?.Name ?? "",
            product.Images.OrderBy(i => i.Order).Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.Order)),
            product.CreatedAt, product.UpdatedAt);
    }
}