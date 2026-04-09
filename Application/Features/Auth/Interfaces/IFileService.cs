using Microsoft.AspNetCore.Http;

namespace StayHub.Backend.Application.Features.Auth.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    void DeleteFile(string filePath);
}
