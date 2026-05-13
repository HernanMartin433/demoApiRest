using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using CartEntity = MiProducto.Domain.Entities.Cart;
using CartItemEntity = MiProducto.Domain.Entities.CartItem;

namespace MiProducto.Application.Features.Cart.Commands;

public record AddToCartCommand(Guid UserId, Guid ProductId, int Quantity = 1) : IRequest<CartDto>;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddToCartCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null) throw new KeyNotFoundException("Producto no encontrado.");

        var cart = await _unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);

        if (cart is null)
        {
            cart = CartEntity.Create(request.UserId);
            await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            cart = await _unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);
        }

        var existingItem = cart!.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + request.Quantity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var item = CartItemEntity.Create(cart.Id, request.ProductId, product.Price, request.Quantity);
            await _unitOfWork.Carts.AddItemAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var updatedCart = await _unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);
        var items = updatedCart!.Items.Select(i => new CartItemDto(
            i.Id, i.ProductId,
            i.Product?.Name ?? "",
            i.Product?.Images?.OrderBy(img => img.Order).FirstOrDefault()?.ImageUrl,
            i.UnitPrice, i.Quantity,
            i.UnitPrice * i.Quantity));

        return new CartDto(updatedCart.Id, items, items.Sum(i => i.Subtotal), items.Sum(i => i.Quantity));
    }
}