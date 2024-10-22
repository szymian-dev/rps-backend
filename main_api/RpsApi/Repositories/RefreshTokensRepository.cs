using RpsApi.Database;
using RpsApi.Models.Database;
using RpsApi.Models.Interfaces.IRepositories;

namespace RpsApi.Repositories;

public class RefreshTokensRepository(ApplicationDbContext dbContext) : IRefreshTokensRepository
{
    public bool AddRefreshToken(RefreshToken token)
    {
        token.CreatedAt = DateTime.UtcNow;
        token.UpdatedAt = DateTime.UtcNow;
        dbContext.RefreshTokens.Add(token);
        int changes = dbContext.SaveChanges();
        return changes > 0;
    }

    public bool ReplaceRefreshToken(RefreshToken token, Guid newToken, DateTime expires)
    {
        token.Token = newToken;
        token.ExpiresAt = expires;
        token.UpdatedAt = DateTime.UtcNow;
        int changes = dbContext.SaveChanges();
        return changes > 0;
    }

    public RefreshToken? GetRefreshToken(User user, Guid deviceId)
    {
        return dbContext.RefreshTokens.SingleOrDefault(t => t.UserId == user.Id && t.DeviceId == deviceId);
    }

    public bool DeleteRefreshToken(RefreshToken token)
    {
        dbContext.RefreshTokens.Remove(token);
        int changes = dbContext.SaveChanges();
        return changes > 0;
    }

    public bool DeleteAllRefreshTokens(User user)
    {
        var userTokens = dbContext.RefreshTokens.Where(t => t.UserId == user.Id);
        if (!userTokens.Any())
        {
            return false;
        }
        dbContext.RefreshTokens.RemoveRange(userTokens);
        int changes = dbContext.SaveChanges();
        return changes > 0;
    }
}