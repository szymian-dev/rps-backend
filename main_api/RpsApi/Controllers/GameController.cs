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
    [Authorize]
    [HttpPost("")]
    public NewGameResponse PostGame(NewGameRequest request)
    {
        return gameService.StartNewGame(request);
    }

    [Authorize]
    [HttpGet("{gameId}")]
    public GameInfoDto GetGameInfo(int gameId)
    {
        return gameService.GetGameInfo(gameId);
    }

    [Authorize]
    [HttpGet("")]
    public PagedResponse<GameInfoDto> GetUsersGames([FromQuery] GamesRequest request)
    {
        return gameService.GetUsersGames(request);
    }

    [Authorize]
    [HttpPost("invitation")]
    public bool PostInvitation(HandleInvitationRequest request)
    {
        return gameService.HandleInvitation(request);
    }
}