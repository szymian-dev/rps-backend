using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Models.Settings;
using RpsApi.Utils;

namespace RpsApi.Services;

public class FileManagementService(IOptions<FileSettings> settings) : IFileManagementService
{
    private readonly FileSettings _fileSettings = ValidateSettings(settings.Value);
    
    public string UploadFile(IFormFile file)
    {
        var fileInfo = ValidateUploadedFile(file);
        using var stream = new FileStream(fileInfo.Path, FileMode.Create);
        file.CopyTo(stream);
        return fileInfo.Name;
    }
    
    public string GetUploadDirectoryPath()
    {
        string dirPath = Path.Combine(Directory.GetCurrentDirectory(), _fileSettings.UploadPath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        return dirPath;
    }

    public bool DeleteFile(string fileName)
    {
        string path = Path.Combine(GetUploadDirectoryPath(), fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }

    public FileStreamResult GetFile(string fileName)
    {
        string path = Path.Combine(GetUploadDirectoryPath(), fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found");
        }
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new FileStreamResult(stream, MimeTypeHelper.GetImageMimeTypeString(fileName));
    }

    private NewFileInfo ValidateUploadedFile(IFormFile file)
    {
        string extension = Path.GetExtension(file.FileName);
        if (!_fileSettings.AllowedExtensions.Contains(extension))
        {
            throw new FileExtensionNotAllowedException("File type not allowed");
        }
        if (file.Length > _fileSettings.MaxFileSize)
        {
            throw new FileSizeExceededException("File size exceeded");
        }
        
        string fileName = Guid.NewGuid() + extension;
        string uploadPath = GetUploadDirectoryPath();
        string path = Path.Combine(uploadPath, fileName);
        return new NewFileInfo
        {
            Name = fileName,
            Extension = extension,
            Path = path,
            Size = file.Length
        };
    }
    
    private static FileSettings ValidateSettings(FileSettings? fileSettings)
    {
        if(fileSettings is null || fileSettings.MaxFileSize == 0 || !fileSettings.AllowedExtensions.Any() || string.IsNullOrEmpty(fileSettings.UploadPath))
        {
            throw new SettingsNotFoundException("File settings not found");
        }
        return fileSettings;
    }
}