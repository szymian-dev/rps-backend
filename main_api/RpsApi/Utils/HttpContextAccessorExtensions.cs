namespace RpsApi.Utils;

using Microsoft.AspNetCore.Http;

public static class HttpContextAccessorExtensions
{
    public static string? GetJwtToken(this IHttpContextAccessor httpContextAccessor)
    {
        var context = httpContextAccessor.HttpContext;
        if (context != null && context.Request.Headers.TryGetValue("Authorization", out var token))
        {
            if (token.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return token.ToString().Substring("Bearer ".Length).Trim();
            }
        }

        return null;
    }
}