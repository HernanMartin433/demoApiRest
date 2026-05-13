using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Orders.Commands;

public record ProcessPaymentCommand(
    Guid OrderId,
    Guid UserId,
    string PaymentMethod,
    bool SimulateApproved
) : IRequest<OrderDto>;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public ProcessPaymentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<OrderDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) throw new KeyNotFoundException("Pedido no encontrado.");
        if (order.UserId != request.UserId) throw new UnauthorizedAccessException("No autorizado.");

        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, out var method))
            throw new ArgumentException("Método de pago inválido.");

        order.ProcessPayment(method, request.SimulateApproved);
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