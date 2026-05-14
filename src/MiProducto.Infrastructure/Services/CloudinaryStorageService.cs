using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MiProducto.Application.Common.Interfaces;

namespace MiProducto.Infrastructure.Services;

public class CloudinaryStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(string cloudName, string apiKey, string apiSecret)
    {
        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> SaveImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "miproducto",
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            throw new Exception($"Error al subir imagen: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }

    public void DeleteImage(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var idx = Array.IndexOf(segments, "miproducto");
            if (idx >= 0 && idx + 1 < segments.Length)
            {
                var publicId = $"miproducto/{Path.GetFileNameWithoutExtension(segments[idx + 1])}";
                _cloudinary.Destroy(new DeletionParams(publicId));
            }
        }
        catch { }
    }
}
//