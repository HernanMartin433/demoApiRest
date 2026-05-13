namespace MiProducto.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    void DeleteImage(string imageUrl);
}