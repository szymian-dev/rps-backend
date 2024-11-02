using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;

namespace RpsApi.Models.Interfaces.IServices;

public interface IStatsService
{
    PlayerStatsDto GetPlayerStats();
    PagedResponse<PlayerStatsDto> GetLeaderboard(LeaderboardRequest request);
    bool UpdatePlayerStats(StatsUpdate request);
}