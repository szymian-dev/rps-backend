using RpsApi.Models.DataTransferObjects;

namespace RpsApi.Models.Interfaces.IServices;

public interface IGestureService
{
    Task<bool> UploadGesture(IFormFile file, int gameId);
    Task<GestureDto> GetGesture(int fileId);
}