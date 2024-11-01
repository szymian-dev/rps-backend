using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Controllers;

/// <summary>
///     Controller for game related actions.
/// </summary>
[ApiController]
[Route("games")]
public class GameController(IGameService gameService)
{
    /// <summary>
    ///     Starts a new game. Challenges another player to a game.
    /// </summary>
    /// <param name="request">
    ///     Request with the id of the player to challenge.
    /// </param>
    /// <returns>
    ///    Response with the information about the new game.
    /// </returns>
    /// <response code="200"> Game started successfully. </response>
    /// <response code="400"> Player with that id does not exist. </response>
    /// <response code="401"> Unauthorized. </response>
    [Authorize]
    [HttpPost("")]
    public ApiResponse<NewGameResponse> PostGame(NewGameRequest request)
    {
        return new ApiResponse<NewGameResponse>()
        {
            Data = gameService.StartNewGame(request),
            Message = "Game started successfully. Returns full information about the new game."
        };
    }
    
    /// <summary>
    ///     Gets full information about a game.
    /// </summary>
    /// <param name="gameId">
    ///     Id of the game.
    /// </param>
    /// <returns>
    ///     Response with the information about the game.
    /// </returns>
    /// <response code="200"> Game info retrieved successfully. </response>
    /// <response code="401"> Unauthorized. </response>
    /// <response code="404"> Game with that id does not exist. </response>
    [Authorize]
    [HttpGet("{gameId}")]
    public ApiResponse<GameInfoDto> GetGameInfo(int gameId)
    {
        return new ApiResponse<GameInfoDto>()
        {
            Data = gameService.GetGameInfo(gameId),
            Message = "Game info retrieved successfully."
        };
    }

    /// <summary>
    ///     Gets a list of games that the user is participating in.
    /// </summary>
    /// <param name="request">
    ///    Request with page number, page size and search filters.
    /// </param>
    /// <returns>
    ///   Response with a filtered list of games.
    /// </returns>
    /// <response code="200"> Games retrieved successfully. </response>
    /// <response code="401"> Unauthorized. </response>
    [Authorize]
    [HttpGet("")]
    public ApiResponse<PagedResponse<GameInfoDto>> GetUsersGames([FromQuery] GamesRequest request)
    {
        return new ApiResponse<PagedResponse<GameInfoDto>>()
        {
            Data = gameService.GetUsersGames(request),
            Message = "Games retrieved successfully. Returns a filtered list of games that the user is participating in."
        };
    }

    /// <summary>
    ///     Handles an invitation to a game.
    /// </summary>
    /// <param name="request">
    ///    Request with the id of the game to accept or decline.
    /// </param>
    /// <returns>
    ///    True if the invitation was handled successfully.
    /// </returns>
    /// <response code="200"> Invitation handled successfully. </response>
    /// <response code="400"> Game with that id does not exist. Game has already started or is cancelled </response>
    /// <response code="401"> Unauthorized </response>
    /// <response code="403"> User is not a participant of that particular game </response>
    [Authorize]
    [HttpPost("invitation")]
    public ApiResponse<bool> PostInvitation(HandleInvitationRequest request)
    {
        return new ApiResponse<bool>()
        {
            Data = gameService.HandleInvitation(request),
            Message = "Invitation handled successfully."
        };
    }
}