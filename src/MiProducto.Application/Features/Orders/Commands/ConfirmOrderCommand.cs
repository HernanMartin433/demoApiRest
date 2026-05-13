using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Orders.Commands;

public record ConfirmOrderCommand(Guid UserId) : IRequest<OrderDto>;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public ConfirmOrderCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<OrderDto> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart is null || !cart.Items.Any())
            throw new InvalidOperationException("El carrito está vacío.");

        var order = Order.Create(request.UserId, cart.Items);
        order.Confirm();
        await _unitOfWork.Orders.AddAsync(order, cancellationToken);

        cart.Items.Clear();
        _unitOfWork.Carts.Update(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var items = order.Items.Select(i => new OrderItemDto(
            i.Id, i.ProductId, i.ProductName,
            i.UnitPrice, i.Quantity, i.UnitPrice * i.Quantity));

        return new OrderDto(
            order.Id, order.Status.ToString(),
            order.PaymentStatus.ToString(),
            order.PaymentMethod?.ToString(),
            order.PaidAt,
            order.Total,
            order.Items.Select(i => new OrderItemDto(
                i.Id, i.ProductId, i.ProductName,
                i.UnitPrice, i.Quantity, i.UnitPrice * i.Quantity)),
            order.CreatedAt);
    }
}