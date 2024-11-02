namespace RpsApi.Models.DataTransferObjects;

public class NewFileInfo
{
    public long Size { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Extension { get; set; } = String.Empty;
    public string Path { get; set; } = String.Empty;
}