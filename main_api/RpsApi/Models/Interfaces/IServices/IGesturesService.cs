using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;

namespace RpsApi.Models.Interfaces.IServices;

public interface IGesturesService
{
    Task<GameUpdateResponse> UploadGesture(IFormFile file, int gameId);
    FileStreamResult GetGesture(int fileId);
    bool DeleteAllUserGestures(User user);
}