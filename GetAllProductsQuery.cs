using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Products.Queries;

// Query
public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

// Handler
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ProductDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);

    return products.Select(p => new ProductDto(
        p.Id, p.Name, p.Description,
        p.Price, p.Stock, p.IsActive,
        p.Images.OrderBy(i => i.Order).Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.Order)),
        p.CreatedAt, p.UpdatedAt));
    }
}
