using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiProducto.Application.Features.Cart.Commands;
using MiProducto.Application.Features.Cart.Queries;
using System.Security.Claims;

namespace MiProducto.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator) => _mediator = mediator;

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
        => Ok(await _mediator.Send(new GetCartQuery(GetUserId()), ct));

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new AddToCartCommand(GetUserId(), request.ProductId, request.Quantity), ct);
        return Ok(result);
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid itemId, [FromBody] UpdateItemRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateCartItemCommand(GetUserId(), itemId, request.Quantity), ct);
        return Ok(result);
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoveFromCartCommand(GetUserId(), itemId), ct);
        return Ok(result);
    }
}

public record AddItemRequest(Guid ProductId, int Quantity = 1);
public record UpdateItemRequest(int Quantity);