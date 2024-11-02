using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IRepositories;

public interface IStatsRepository
{
    PlayerStats? GetPlayerStats(int userId);
    IQueryable<PlayerStats> GetLeaderboard();
    bool UpdatePlayerStats(PlayerStats stats);
    bool AddPlayerStats(PlayerStats stats);
}