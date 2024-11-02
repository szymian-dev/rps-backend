using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Utils;

namespace RpsApi.Services;

public class StatsService(IStatsRepository statsRepository, IUserContextService userContextService) : IStatsService
{
    public PlayerStatsDto GetPlayerStats()
    {
        var user = userContextService.GetCurrentUser();
        var stats = statsRepository.GetPlayerStats(user.Id);
        if (stats is null)
        {
            return EmptyPlayerStatsDto(user);
        }
        return MapPlayerStatsToDto(stats);
    }

    public PagedResponse<PlayerStatsDto> GetLeaderboard(LeaderboardRequest request)
    {
        var stats = statsRepository.GetLeaderboard();
        if (!stats.Any())
        {
            return PaginationHelper.EmptyResponse<PlayerStatsDto>(request.PageSize);
        }
        var calculatedPagination = PaginationHelper.CalculatePagination(stats, request.PageNumber, request.PageSize);
        var leaderboard = stats
            .OrderByDescending(s => s.Wins)
            .ThenByDescending(s => s.Ties)
            .ThenBy(s => s.Losses)
            .ThenByDescending(s => s.GamesPlayed)
            .ApplyPagination(calculatedPagination.PageNumber, calculatedPagination.PageSize)
            .Select(s => MapPlayerStatsToDto(s))
            .ToList();
        return new PagedResponse<PlayerStatsDto>
        {
            Items = leaderboard,
            PageNumber = calculatedPagination.PageNumber,
            PageSize = calculatedPagination.PageSize,
            TotalCount = calculatedPagination.TotalCount,
            TotalPages = calculatedPagination.TotalPages
        };
    }

    public bool UpdatePlayerStats(StatsUpdate request)
    {
        var stats = statsRepository.GetPlayerStats(request.UserId) ?? AddNewPlayerStats(request.UserId);

        switch (request.WinStatus)
        {
            case WinStatus.Won:
                stats.Wins++;
                break;
            case WinStatus.Lost:
                stats.Losses++;
                break;
            case WinStatus.Draw:
                stats.Ties++;
                break;
            default:
                throw new InvalidGameStateException("Invalid win status");
        }
        stats.GamesPlayed++;
        return statsRepository.UpdatePlayerStats(stats);
    }
    
    private static PlayerStatsDto MapPlayerStatsToDto(PlayerStats stats)
    {
        if (stats.User is null)
        {
            throw new NotFoundException("User not found");
        }
        return new PlayerStatsDto
        {
            Wins = stats.Wins,
            Losses = stats.Losses,
            Ties = stats.Ties,
            GamesPlayed = stats.GamesPlayed,
            PlayerInfo = new UserResponse
            {
                Id = stats.User.Id,
                Username = stats.User.Username,
                Email = stats.User.Email
            }
        };
    }
    private static PlayerStatsDto EmptyPlayerStatsDto(User user)
    {
        return new PlayerStatsDto
        {
            Wins = 0,
            Losses = 0,
            Ties = 0,
            GamesPlayed = 0,
            PlayerInfo = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        };
    }
    
    private static PlayerStats NewPlayerStats(int userId)
    {
        return new PlayerStats
        {
            UserId = userId,
            Wins = 0,
            Losses = 0,
            Ties = 0,
            GamesPlayed = 0
        };
    }
    
    private PlayerStats AddNewPlayerStats(int userId)
    {
        var stats = NewPlayerStats(userId);
        bool succ = statsRepository.AddPlayerStats(stats);
        if (!succ)
        {
            throw new DatabaseException("Failed to add new player stats");
        }
        return stats;
    }
}