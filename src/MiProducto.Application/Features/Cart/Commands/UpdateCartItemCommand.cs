using MiProducto.Domain.Entities;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Cart.Commands;

public record UpdateCartItemCommand(Guid UserId, Guid ItemId, int Quantity) : IRequest<CartDto>;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCartItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart is null) throw new KeyNotFoundException("Carrito no encontrado.");

        var item = cart.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item is null) throw new KeyNotFoundException("Item no encontrado.");

        if (request.Quantity <= 0)
            cart.Items.Remove(item);
        else
            item.UpdateQuantity(request.Quantity);

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