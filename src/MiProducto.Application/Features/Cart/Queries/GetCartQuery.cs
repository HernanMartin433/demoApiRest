using MiProducto.Domain.Entities;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Cart.Queries;

public record GetCartQuery(Guid UserId) : IRequest<CartDto>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetCartQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);

        if (cart is null)
            return new CartDto(Guid.Empty, Enumerable.Empty<CartItemDto>(), 0, 0);

        var items = cart.Items.Select(i => new CartItemDto(
            i.Id, i.ProductId,
            i.Product?.Name ?? "",
            i.Product?.Images?.OrderBy(img => img.Order).FirstOrDefault()?.ImageUrl,
            i.UnitPrice, i.Quantity,
            i.UnitPrice * i.Quantity));

        return new CartDto(cart.Id, items, items.Sum(i => i.Subtotal), items.Sum(i => i.Quantity));
    }
}