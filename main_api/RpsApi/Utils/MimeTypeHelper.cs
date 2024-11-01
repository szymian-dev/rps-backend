using System.Net.Http.Headers;

namespace RpsApi.Utils;

public static class MimeTypeHelper
{
    public static string GetImageMimeTypeString(string fileName)
    {
        return Path.GetExtension(fileName).ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
    
    public static MediaTypeHeaderValue GetImageMimeType(string fileName)
    {
        return MediaTypeHeaderValue.Parse(GetImageMimeTypeString(fileName));
    }
}