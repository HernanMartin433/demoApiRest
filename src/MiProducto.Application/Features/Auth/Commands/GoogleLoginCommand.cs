using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Auth.Commands;

public record GoogleLoginCommand(
    string GoogleId,
    string Email,
    string FullName
) : IRequest<AuthResponse>;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public GoogleLoginCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        // Buscar por GoogleId
        var user = await _unitOfWork.Users.GetByGoogleIdAsync(request.GoogleId, cancellationToken);

        if (user is null)
        {
            // Buscar por email — puede que ya tenga cuenta manual
            user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                // Crear nuevo usuario
                user = User.CreateFromGoogle(request.Email, request.FullName, request.GoogleId);
                await _unitOfWork.Users.AddAsync(user, cancellationToken);
            }
            else
            {
                // Vincular Google a cuenta existente
                user.LinkGoogle(request.GoogleId);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var token = _jwtService.GenerateToken(user);
        var refreshToken = RefreshToken.Create(user.Id);
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(token, refreshToken.Token, user.Email, user.FullName, user.Role);
    }
}