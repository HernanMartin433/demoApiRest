using FluentValidation;
using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Auth.Commands;

public record RegisterCommand(string FullName, string Email, string Password, string Role = "User") : IRequest<AuthResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage("El nombre es requerido.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("El email no es válido.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
        RuleFor(x => x.Role)
            .Must(r => r == "Admin" || r == "User")
            .WithMessage("El rol debe ser Admin o User.");
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Users.ExistsByEmailAsync(request.Email, cancellationToken);
        if (exists)
            throw new InvalidOperationException("El email ya está registrado.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FullName, request.Role);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtService.GenerateToken(user);
        var refreshToken = RefreshToken.Create(user.Id);
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(token, refreshToken.Token, user.Email, user.FullName, user.Role);
    }
}