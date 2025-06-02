using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;

namespace RpsApi.Services;

public interface IJwtService
{
    JwtTokenDto CreateJwtForUser(User user);
    RefreshTokenDto CreateOrReplaceRefreshToken(User user, Guid deviceId);
    AuthDto RefreshTokens(RefreshRequest request, Guid refreshToken);
    bool RevokeRefreshToken(User user, Guid deviceId);
    bool RevokeAllRefreshTokens(User user);
    JwtTokenDto GetJwtForAiModelApi();
    JwtTokenDto CreateJwtForAiModelApi();
}