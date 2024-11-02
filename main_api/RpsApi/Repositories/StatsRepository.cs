using Microsoft.EntityFrameworkCore;
using RpsApi.Database;
using RpsApi.Models.Database;
using RpsApi.Models.Interfaces.IRepositories;

namespace RpsApi.Repositories;

public class StatsRepository(ApplicationDbContext dbContext) : IStatsRepository
{
    public PlayerStats? GetPlayerStats(int userId)
    {
        return dbContext.PlayerStats
            .Include(x => x.User)
            .SingleOrDefault(x => x.UserId == userId);
    }

    public IQueryable<PlayerStats> GetLeaderboard()
    {
        return dbContext.PlayerStats
            .Include(x => x.User);
    }

    public bool UpdatePlayerStats(PlayerStats stats)
    {
        stats.UpdatedAt = DateTime.UtcNow;
        dbContext.PlayerStats.Update(stats);
        return dbContext.SaveChanges() > 0;
    }

    public bool AddPlayerStats(PlayerStats stats)
    {
        stats.UpdatedAt = DateTime.UtcNow;
        stats.CreatedAt = DateTime.UtcNow;
        dbContext.PlayerStats.Add(stats);
        return dbContext.SaveChanges() > 0;
    }
}