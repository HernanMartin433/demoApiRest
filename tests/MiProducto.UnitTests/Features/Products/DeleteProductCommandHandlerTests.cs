using FluentAssertions;
using Moq;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.Features.Products.Commands;
using MiProducto.Domain.Entities;
using MiProducto.UnitTests.Helpers;

namespace MiProducto.UnitTests.Features.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteProductCommandHandler _handler;

    private static readonly Guid _rubroId = Guid.NewGuid();
    private static readonly Guid _subrubroId = Guid.NewGuid();

    public DeleteProductCommandHandlerTests()
    {
        _unitOfWork = MockUnitOfWork.Create();
        _handler = new DeleteProductCommandHandler(_unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenProductExists()
    {
        // Arrange
        var product = Product.Create("Producto", "Desc", 100m, 10, _rubroId, _subrubroId);

        _unitOfWork.Setup(u => u.Products.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _unitOfWork.Verify(u => u.Products.Delete(product), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenProductDoesNotExist()
    {
        // Arrange
        _unitOfWork.Setup(u => u.Products.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.Handle(new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _unitOfWork.Verify(u => u.Products.Delete(It.IsAny<Product>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}