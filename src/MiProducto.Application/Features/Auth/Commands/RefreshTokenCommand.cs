using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;

namespace MiProducto.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _unitOfWork.RefreshTokens
            .GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is null || !refreshToken.IsValid)
            throw new UnauthorizedAccessException("Refresh token inválido o expirado.");

        var user = await _unitOfWork.Users.GetByEmailAsync(
            refreshToken.User.Email, cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Usuario no encontrado.");

        refreshToken.Revoke();
        _unitOfWork.RefreshTokens.Update(refreshToken);

        var newRefreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

        var newToken = _jwtService.GenerateToken(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(newToken, newRefreshToken.Token, user.Email, user.FullName, user.Role);
    }
}