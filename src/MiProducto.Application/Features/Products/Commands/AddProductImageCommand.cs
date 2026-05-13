using MediatR;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Application.DTOs;
using MiProducto.Domain.Entities;

namespace MiProducto.Application.Features.Products.Commands;

public record AddProductImageCommand(
    Guid ProductId,
    Stream FileStream,
    string FileName
) : IRequest<ProductDto?>;

public class AddProductImageCommandHandler : IRequestHandler<AddProductImageCommand, ProductDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;

    public AddProductImageCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<ProductDto?> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdWithImagesAsync(request.ProductId, cancellationToken);
        if (product is null) return null;

        var count = await _unitOfWork.ProductImages.CountByProductIdAsync(request.ProductId, cancellationToken);
        if (count >= 5)
            throw new InvalidOperationException("El producto ya tiene el máximo de 5 imágenes.");

        var imageUrl = await _fileStorage.SaveImageAsync(request.FileStream, request.FileName, cancellationToken);
        var image = ProductImage.Create(request.ProductId, imageUrl, count + 1);

        await _unitOfWork.ProductImages.AddAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _unitOfWork.Products.GetByIdWithImagesAsync(request.ProductId, cancellationToken);

        return new ProductDto(
            updatedProduct!.Id, updatedProduct.Name, updatedProduct.Description,
            updatedProduct.Price, updatedProduct.Stock, updatedProduct.IsActive,
            updatedProduct.RubroId, updatedProduct.Rubro?.Name ?? "",
            updatedProduct.SubrubroId, updatedProduct.Subrubro?.Name ?? "",
            updatedProduct.Images.OrderBy(i => i.Order).Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.Order)),
            updatedProduct.CreatedAt, updatedProduct.UpdatedAt);
    }
}