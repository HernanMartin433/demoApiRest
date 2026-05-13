using MediatR;
using MiProducto.Application.Common.Interfaces;

namespace MiProducto.Application.Features.Products.Commands;

public record DeleteProductImageCommand(Guid ImageId) : IRequest<bool>;

public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;

    public DeleteProductImageCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<bool> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _unitOfWork.ProductImages.GetByIdAsync(request.ImageId, cancellationToken);
        if (image is null) return false;

        _fileStorage.DeleteImage(image.ImageUrl);
        _unitOfWork.ProductImages.Delete(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}