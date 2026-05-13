using FluentAssertions;
using Moq;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.Features.Products.Queries;
using MiProducto.Domain.Entities;
using MiProducto.UnitTests.Helpers;

namespace MiProducto.UnitTests.Features.Products;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly GetAllProductsQueryHandler _handler;

    private static readonly Guid _rubroId = Guid.NewGuid();
    private static readonly Guid _subrubroId = Guid.NewGuid();

    public GetAllProductsQueryHandlerTests()
    {
        _unitOfWork = MockUnitOfWork.Create();
        _handler = new GetAllProductsQueryHandler(_unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResult_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Producto A", "Descripcion A", 100, 10, _rubroId, _subrubroId),
            Product.Create("Producto B", "Descripcion B", 200, 20, _rubroId, _subrubroId),
        };

        _unitOfWork.Setup(u => u.Products.GetPagedAsync(1, 6, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 2));

        // Act
        var result = await _handler.Handle(new GetAllProductsQuery(1, 6), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyResult_WhenNoProductsExist()
    {
        // Arrange
        _unitOfWork.Setup(u => u.Products.GetPagedAsync(1, 6, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        // Act
        var result = await _handler.Handle(new GetAllProductsQuery(1, 6), CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CorrectlyCalculatesTotalPages()
    {
        // Arrange
        var products = Enumerable.Range(1, 6)
            .Select(i => Product.Create($"Producto {i}", $"Desc {i}", i * 100, i, _rubroId, _subrubroId))
            .ToList();

        _unitOfWork.Setup(u => u.Products.GetPagedAsync(1, 6, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 12));

        // Act
        var result = await _handler.Handle(new GetAllProductsQuery(1, 6), CancellationToken.None);

        // Assert
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }
}