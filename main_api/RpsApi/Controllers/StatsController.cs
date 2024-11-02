using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Controllers;
/// <summary>
///     Controller for statistics related actions e.g. getting statistics about the user, leaderboard.
/// </summary>
[ApiController]
[Route("statistics")]
[Authorize]
public class StatsController(IStatsService statsService)
{
    /// <summary>
    ///     Get the player statistics for the current user.
    /// </summary>
    /// <returns>
    ///    Player stats object containing the player statistics.
    /// </returns>
    /// <response code="200">Player statistics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("me")]
    public ApiResponse<PlayerStatsDto> GetPlayerStats()
    {
        return new ApiResponse<PlayerStatsDto>()
        {
            Data = statsService.GetPlayerStats(),
            Message = "Player statistics retrieved successfully"
        };
    }
    
    /// <summary>
    ///     Get the leaderboard of players.
    /// </summary>
    /// <param name="request">
    ///     Request object containing the page number, page size, sort by and order.
    /// </param>
    /// <returns>
    ///     Paged response containing the leaderboard of players.
    /// </returns>
    /// <response code="200">Leaderboard retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("")]
    public ApiResponse<PagedResponse<PlayerStatsDto>> GetLeaderboard([FromQuery] LeaderboardRequest request)
    {
        return new ApiResponse<PagedResponse<PlayerStatsDto>>()
        {
            Data = statsService.GetLeaderboard(request),
            Message = "Leaderboard retrieved successfully"
        };
    }
}