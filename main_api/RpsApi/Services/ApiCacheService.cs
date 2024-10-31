using Microsoft.Extensions.Caching.Memory;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Services;

public class ApiCacheService(IMemoryCache memoryCache) : IApiCacheService
{
    private readonly string tokenKey = "ApiToken";
    public void SaveToken(JwtTokenDto tokenDto)
    {
        memoryCache.Set(tokenKey, tokenDto);
    }
    public JwtTokenDto? RetrieveToken()
    {
        return memoryCache.Get<JwtTokenDto>(tokenKey);
    }
    public void RemoveToken()
    {
        memoryCache.Remove(tokenKey);
    }
}