using Microsoft.AspNetCore.Mvc;

namespace RpsApi.Models.Interfaces.IServices;

public interface IFileManagementService
{
    string UploadFile(IFormFile file);
    string GetUploadDirectoryPath();
    bool DeleteFile(string fileName);
    FileStreamResult GetFile(string fileName);
}