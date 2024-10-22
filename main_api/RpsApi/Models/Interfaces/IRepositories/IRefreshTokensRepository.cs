using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IRepositories;

public interface IRefreshTokensRepository
{
    bool AddRefreshToken(RefreshToken token);
    bool ReplaceRefreshToken(RefreshToken token, Guid newToken, DateTime expires);
    RefreshToken? GetRefreshToken(User user, Guid deviceId);
    bool DeleteRefreshToken(RefreshToken token);
    bool DeleteAllRefreshTokens(User user);
}