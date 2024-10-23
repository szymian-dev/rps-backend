using Microsoft.AspNetCore.Mvc;
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
    [HttpPost("")]
    public NewGameResponse PostGame(NewGameRequest request)
    {
        return gameService.StartNewGame(request);
    }
}