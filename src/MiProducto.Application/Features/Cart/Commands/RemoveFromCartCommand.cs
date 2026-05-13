using MiProducto.Domain.Entities;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Cart.Commands;

public record RemoveFromCartCommand(Guid UserId, Guid ItemId) : IRequest<CartDto>;

public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public RemoveFromCartCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CartDto> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart is null) throw new KeyNotFoundException("Carrito no encontrado.");

        var item = cart.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item is not null) cart.Items.Remove(item);

        _unitOfWork.Carts.Update(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var items = cart.Items.Select(i => new CartItemDto(
            i.Id, i.ProductId,
            i.Product?.Name ?? "",
            i.Product?.Images?.OrderBy(img => img.Order).FirstOrDefault()?.ImageUrl,
            i.UnitPrice, i.Quantity,
            i.UnitPrice * i.Quantity));

        return new CartDto(cart.Id, items, items.Sum(i => i.Subtotal), items.Sum(i => i.Quantity));
    }
}