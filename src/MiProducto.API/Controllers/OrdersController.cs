using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiProducto.Application.Features.Orders.Commands;
using MiProducto.Application.Features.Orders.Queries;
using System.Security.Claims;


namespace MiProducto.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken ct)
        => Ok(await _mediator.Send(new GetOrdersQuery(GetUserId()), ct));

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmOrder(CancellationToken ct)
    {
        var result = await _mediator.Send(new ConfirmOrderCommand(GetUserId()), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> ProcessPayment(
        Guid id,
        [FromBody] ProcessPaymentRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
        new ProcessPaymentCommand(id, GetUserId(), request.PaymentMethod, request.SimulateApproved), ct);
        return Ok(result);
    }
    public record ProcessPaymentRequest(string PaymentMethod, bool SimulateApproved);

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id, GetUserId()), ct);
        return Ok(result);
    }
}