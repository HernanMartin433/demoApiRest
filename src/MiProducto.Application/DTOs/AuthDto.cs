namespace MiProducto.Application.DTOs;

public record RegisterRequest(string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string RefreshToken, string Email, string FullName, string Role);
public record RefreshTokenRequest(string RefreshToken);