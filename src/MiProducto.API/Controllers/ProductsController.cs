using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiProducto.Application.Features.Products.Commands;
using MiProducto.Application.Features.Products.Queries;

namespace MiProducto.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene todos los productos activos con paginación.</summary>

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 6,
        [FromQuery] string? search = null,
        [FromQuery] Guid? rubroId = null,
        [FromQuery] Guid? subrubroId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAllProductsQuery(pageNumber, pageSize, search, rubroId, subrubroId),
            cancellationToken);
        return Ok(result);
    }   

    /// <summary>Obtiene un producto por ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Crea un nuevo producto. Solo Admin.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Agrega una imagen al producto. Solo Admin.</summary>
    [HttpPost("{id:guid}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage(
        Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "El archivo es requerido." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
            return BadRequest(new { message = "Solo se permiten JPG, PNG o WebP." });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "La imagen no puede superar 5MB." });

        // Copiamos el stream a memoria para evitar que se cierre
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var result = await _mediator.Send(
            new AddProductImageCommand(id, memoryStream, file.FileName), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Elimina una imagen. Solo Admin.</summary>
    [HttpDelete("images/{imageId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductImageCommand(imageId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    /// <summary>Actualiza un producto. Solo Admin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Elimina un producto. Solo Admin.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}