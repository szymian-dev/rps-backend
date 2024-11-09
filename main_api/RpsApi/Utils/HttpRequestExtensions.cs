namespace RpsApi.Utils;

public static class HttpRequestExtensions
{
    public static string? GetRefreshTokenFromCookies(this HttpRequest request)
    {
        if (request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return refreshToken;
        }
        return null; 
    }
}