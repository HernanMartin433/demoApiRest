/*using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Products.Queries;

public record GetAllProductsQuery(int PageNumber = 1, int PageSize = 5) : IRequest<PagedResult<ProductDto>>;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<PagedResult<ProductDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 5 : request.PageSize > 50 ? 50 : request.PageSize;

        var (items, totalCount) = await _unitOfWork.Products
            .GetPagedAsync(pageNumber, pageSize, cancellationToken);

        var dtos = items.Select(p => new ProductDto(
            p.Id, p.Name, p.Description,
            p.Price, p.Stock, p.IsActive,
            p.RubroId, p.Rubro?.Name ?? "",
            p.SubrubroId, p.Subrubro?.Name ?? "",
            p.Images.OrderBy(i => i.Order).Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.Order)),
            p.CreatedAt, p.UpdatedAt));

        return new PagedResult<ProductDto>(dtos, totalCount, pageNumber, pageSize);
    }
}
*/
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Products.Queries;

public record GetAllProductsQuery(
    int PageNumber = 1,
    int PageSize = 6,
    string? Search = null,
    Guid? RubroId = null,
    Guid? SubrubroId = null
) : IRequest<PagedResult<ProductDto>>;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<PagedResult<ProductDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 6 : request.PageSize > 50 ? 50 : request.PageSize;

        var (items, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            pageNumber, pageSize,
            request.Search,
            request.RubroId,
            request.SubrubroId,
            cancellationToken);

        var dtos = items.Select(p => new ProductDto(
            p.Id, p.Name, p.Description,
            p.Price, p.Stock, p.IsActive,
            p.RubroId, p.Rubro?.Name ?? "",
            p.SubrubroId, p.Subrubro?.Name ?? "",
            p.Images.OrderBy(i => i.Order).Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.Order)),
            p.CreatedAt, p.UpdatedAt));

        return new PagedResult<ProductDto>(dtos, totalCount, pageNumber, pageSize);
    }
}