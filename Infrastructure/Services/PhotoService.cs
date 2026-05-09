using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StayHub.Backend.Application.Common.Interfaces;
using StayHub.Backend.Infrastructure.Security;

namespace StayHub.Backend.Infrastructure.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length <= 0) return null;

        // Validation: Cloudinary Free Tier 10MB Limit
        if (file.Length > 10 * 1024 * 1024) // 10,485,760 bytes
        {
            throw new ArgumentException("File exceeds the 10MB limit.");
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, memoryStream),
            Transformation = new Transformation()
                .Height(800)
                .Width(800)
                .Crop("limit")
                .Quality("auto")
                .FetchFormat("auto"),
            Folder = "stayhub-assets"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary Upload Error: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.AbsoluteUri;
    }

    public async Task<List<string>> UploadImagesAsync(List<IFormFile> files)
    {
        var urls = new List<string>();

        foreach (var file in files)
        {
            var url = await UploadImageAsync(file);
            if (url != null)
            {
                urls.Add(url);
            }
        }

        return urls;
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok";
    }
}
