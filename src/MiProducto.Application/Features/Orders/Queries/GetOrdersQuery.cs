using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Orders.Queries;

public record GetOrdersQuery(Guid UserId) : IRequest<IEnumerable<OrderDto>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetOrdersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdAsync(request.UserId, cancellationToken);

        return orders.Select(o => new OrderDto(
            o.Id, o.Status.ToString(),
            o.PaymentStatus.ToString(),
            o.PaymentMethod?.ToString(),
            o.PaidAt,
            o.Total,
            o.Items.Select(i => new OrderItemDto(
                i.Id, i.ProductId, i.ProductName,
                i.UnitPrice, i.Quantity, i.UnitPrice * i.Quantity)),
            o.CreatedAt));
    }
}