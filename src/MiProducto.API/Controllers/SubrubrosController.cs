using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiProducto.Application.Features.Subrubros.Commands;
using MiProducto.Application.Features.Subrubros.Queries;

namespace MiProducto.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SubrubrosController : ControllerBase
{
    private readonly IMediator _mediator;
    public SubrubrosController(IMediator mediator) => _mediator = mediator;

    [HttpGet("rubro/{rubroId:guid}")]
    public async Task<IActionResult> GetByRubro(Guid rubroId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSubrubrosByRubroQuery(rubroId), ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSubrubroCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetByRubro), new { rubroId = result.RubroId }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubrubroCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { Id = id }, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSubrubroCommand(id), ct);
        return result ? NoContent() : NotFound();
    }
}