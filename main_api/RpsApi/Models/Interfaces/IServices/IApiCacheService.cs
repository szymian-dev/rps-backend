using RpsApi.Models.DataTransferObjects;

namespace RpsApi.Models.Interfaces.IServices;

public interface IApiCacheService
{
    void SaveToken(JwtTokenDto tokenDto);
    JwtTokenDto? RetrieveToken();
    void RemoveToken();
}