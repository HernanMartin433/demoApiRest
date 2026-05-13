using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using MiProducto.Application.Features.Auth.Commands;
using System.Security.Claims;

namespace MiProducto.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        if (command.Role == "Admin" && !User.IsInRole("Admin"))
            return Forbid();

        var result = await _mediator.Send(command, ct);
        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result ? NoContent() : BadRequest(new { message = "Token inválido." });
    }

    /// <summary>Inicia el flujo de login con Google.</summary>
    [HttpGet("google")]
    public IActionResult GoogleLogin([FromQuery] string returnUrl = "http://localhost:5173")
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback)),
            Items = { { "returnUrl", returnUrl } }
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>Callback de Google OAuth.</summary>
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return Redirect("http://localhost:5173?error=google_failed");

        var googleId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        var name = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;

        if (googleId is null || email is null)
            return Redirect("http://localhost:5173?error=google_failed");

        var authResponse = await _mediator.Send(
            new GoogleLoginCommand(googleId, email, name ?? email));

        var returnUrl = result.Properties?.Items["returnUrl"] ?? "http://localhost:5173";
        return Redirect($"{returnUrl}?token={authResponse.Token}&refreshToken={authResponse.RefreshToken}&email={authResponse.Email}&fullName={Uri.EscapeDataString(authResponse.FullName)}&role={authResponse.Role}");
    }
}