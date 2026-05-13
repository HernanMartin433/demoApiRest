using FluentAssertions;
using Moq;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.Features.Products.Commands;
using MiProducto.Domain.Entities;
using MiProducto.UnitTests.Helpers;

namespace MiProducto.UnitTests.Features.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateProductCommandHandler _handler;

    private static readonly Guid _rubroId = Guid.NewGuid();
    private static readonly Guid _subrubroId = Guid.NewGuid();

    public CreateProductCommandHandlerTests()
    {
        _unitOfWork = MockUnitOfWork.Create();
        _handler = new CreateProductCommandHandler(_unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_CreatesProduct_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateProductCommand("Producto Test", "Descripcion Test", 999.99m, 50, _rubroId, _subrubroId);

        _unitOfWork.Setup(u => u.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Producto Test");
        result.Price.Should().Be(999.99m);
        result.Stock.Should().Be(50);
        result.IsActive.Should().BeTrue();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_CallsSaveChanges_WhenProductIsCreated()
    {
        // Arrange
        var command = new CreateProductCommand("Producto Test", "Descripcion Test", 100m, 10, _rubroId, _subrubroId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsAddAsync_WithCorrectProduct()
    {
        // Arrange
        var command = new CreateProductCommand("Nuevo Producto", "Descripcion", 500m, 25, _rubroId, _subrubroId);
        Product? capturedProduct = null;

        _unitOfWork.Setup(u => u.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => capturedProduct = p)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Name.Should().Be("Nuevo Producto");
        capturedProduct.Price.Should().Be(500m);
    }
}