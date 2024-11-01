using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Controllers;

/// <summary>
///     Controller for gesture related actions e.g. uploading and downloading gestures.
/// </summary>
[ApiController]
[Route("gestures")]
[Authorize]
public class GesturesController(IGesturesService gesturesService)
{
    /// <summary>
    ///     Uploads a gesture file to the server. Analyzes the gesture and saves it to the database.
    /// </summary>
    /// <param name="file">
    ///    File with the gesture.
    /// </param>
    /// <param name="gameId">
    ///   Id of the game that the gesture is for.
    /// </param>
    /// <returns>
    ///   Response with the information about the game.
    /// </returns>
    /// <response code="200"> Gesture uploaded successfully. </response>
    /// <response code="400"> Gesture file is invalid. Game with that id does not exist. Game is not in the gesture phase. User has already uploaded a gesture for this game. </response>
    /// <response code="401"> Unauthorized. </response>
    /// <response code="403"> User is not a participant in the game. </response>
    /// <response code="500"> Internal server error or could not get a response from AI Model API </response>
    [HttpPost("")]
    public async Task<ApiResponse<GameUpdateResponse>> PostGesture(IFormFile file, int gameId)
    {
        return new ApiResponse<GameUpdateResponse>()
        {
            Data = await gesturesService.UploadGesture(file, gameId),
            Message = "Gesture uploaded successfully. Returns information about changes in the game state."
        };
    }
    
    /// <summary>
    ///     Downloads a gesture file from the server.
    /// </summary>
    /// <param name="fileId">
    ///   Id of the gesture file.
    /// </param>
    /// <returns>
    ///  Response with the gesture file.
    /// </returns>
    /// <response code="200"> Gesture file downloaded successfully. </response>
    /// <response code="401"> Unauthorized. </response>
    /// <response code="404"> Gesture file with that id does not exist. </response>
    /// <response code="403"> User is not a participant in the game. </response>
    [HttpGet("{fileId}")]
    public ApiResponse<FileStreamResult> GetGesture(int fileId)
    {
        return new ApiResponse<FileStreamResult>()
        {
            Data = gesturesService.GetGesture(fileId),
            Message = "Gesture file downloaded successfully."
        };
    }
}