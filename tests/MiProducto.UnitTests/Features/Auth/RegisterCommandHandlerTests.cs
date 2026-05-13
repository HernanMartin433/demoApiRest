using FluentAssertions;
using Moq;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.Features.Auth.Commands;
using MiProducto.Domain.Entities;
using MiProducto.UnitTests.Helpers;

namespace MiProducto.UnitTests.Features.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _unitOfWork = MockUnitOfWork.Create();
        _jwtService = new Mock<IJwtService>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _handler = new RegisterCommandHandler(_unitOfWork.Object, _jwtService.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task Handle_RegistersUser_WhenEmailIsNotTaken()
    {
        // Arrange
        _unitOfWork.Setup(u => u.Users.ExistsByEmailAsync("nuevo@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasher.Setup(p => p.Hash("123456")).Returns("hashedPassword");
        _jwtService.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("fake-jwt-token");

        _unitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork.Setup(u => u.RefreshTokens.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RegisterCommand("Nuevo Usuario", "nuevo@test.com", "123456", "User");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("nuevo@test.com");
        result.Token.Should().Be("fake-jwt-token");
        _unitOfWork.Verify(u => u.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperation_WhenEmailAlreadyExists()
    {
        // Arrange
        _unitOfWork.Setup(u => u.Users.ExistsByEmailAsync("existente@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterCommand("Usuario", "existente@test.com", "123456");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("El email ya está registrado.");
    }
}