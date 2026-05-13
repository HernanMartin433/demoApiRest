using FluentAssertions;
using Moq;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.Features.Auth.Commands;
using MiProducto.Domain.Entities;
using MiProducto.UnitTests.Helpers;

namespace MiProducto.UnitTests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _unitOfWork = MockUnitOfWork.Create();
        _jwtService = new Mock<IJwtService>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _handler = new LoginCommandHandler(_unitOfWork.Object, _jwtService.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var user = User.Create("test@test.com", "hashedPassword", "Test User", "Admin");

        _unitOfWork.Setup(u => u.Users.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(p => p.Verify("123456", "hashedPassword")).Returns(true);
        _jwtService.Setup(j => j.GenerateToken(user)).Returns("fake-jwt-token");

        _unitOfWork.Setup(u => u.RefreshTokens.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new LoginCommand("test@test.com", "123456"), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("fake-jwt-token");
        result.Email.Should().Be("test@test.com");
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorized_WhenUserNotFound()
    {
        // Arrange
        _unitOfWork.Setup(u => u.Users.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(
            new LoginCommand("noexiste@test.com", "123456"), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Email o contraseña incorrectos.");
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorized_WhenPasswordIsWrong()
    {
        // Arrange
        var user = User.Create("test@test.com", "hashedPassword", "Test User");

        _unitOfWork.Setup(u => u.Users.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(p => p.Verify("wrongpassword", "hashedPassword")).Returns(false);

        // Act
        var act = async () => await _handler.Handle(
            new LoginCommand("test@test.com", "wrongpassword"), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Email o contraseña incorrectos.");
    }
}