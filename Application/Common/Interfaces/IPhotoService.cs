using Microsoft.AspNetCore.Http;

namespace StayHub.Backend.Application.Common.Interfaces;

public interface IPhotoService
{
    Task<string?> UploadImageAsync(IFormFile file);
    Task<List<string>> UploadImagesAsync(List<IFormFile> files);
    Task<bool> DeleteImageAsync(string publicId);
}
