using MiProducto.Application.Common.Interfaces;

namespace MiProducto.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public FileStorageService(string basePath, string baseUrl)
    {
        _basePath = basePath;
        _baseUrl = baseUrl;
    }

    public async Task<string> SaveImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(fileName).ToLower();
        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(_basePath, uniqueName);

        Directory.CreateDirectory(_basePath);

        using var fs = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fs, cancellationToken);

        return $"{_baseUrl}/{uniqueName}";
    }

    public void DeleteImage(string imageUrl)
    {
        try
        {
            var fileName = Path.GetFileName(imageUrl);
            var fullPath = Path.Combine(_basePath, fileName);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        catch { }
    }
    public async Task DeleteFileAsync(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_basePath, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
        await Task.CompletedTask;
    }
}