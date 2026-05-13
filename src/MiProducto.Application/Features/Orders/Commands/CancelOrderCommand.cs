using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Orders.Commands;

public record CancelOrderCommand(Guid OrderId, Guid UserId) : IRequest<OrderDto>;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public CancelOrderCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) throw new KeyNotFoundException("Pedido no encontrado.");
        if (order.UserId != request.UserId) throw new UnauthorizedAccessException("No autorizado.");
        if (order.PaymentStatus.ToString() == "Approved")
            throw new InvalidOperationException("No se puede cancelar un pedido ya pagado.");

        order.Cancel();
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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