using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Controllers;

[ApiController]
[Route("gestures")]
[Authorize]
public class GesturesController(IGestureService gestureService)
{
    [HttpPost("")]
    public async Task<GameUpdateResponse> PostGesture(IFormFile file, int gameId)
    {
        return await gestureService.UploadGesture(file, gameId);
    }
    
    [HttpGet("{fileId}")]
    public FileStreamResult GetGesture(int fileId)
    {
        return gestureService.GetGesture(fileId);
    }
}