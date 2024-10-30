namespace RpsApi.Models.Settings;

public class FileSettings
{
    public List<string> AllowedExtensions { get; set; } = new List<string>();
    public long MaxFileSize { get; set; }
    public string UploadPath { get; set; } = String.Empty;
}