using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;

namespace RpsApi.Services;

public interface IJwtService
{
    JwtTokenDto CreateJwToken(User user);
    RefreshTokenDto CreateOrReplaceRefreshToken(User user, Guid deviceId);
    AuthResponse RefreshTokens(RefreshRequest request);
    bool RevokeRefreshToken(User user, Guid deviceId);
    bool RevokeAllRefreshTokens(User user);
}