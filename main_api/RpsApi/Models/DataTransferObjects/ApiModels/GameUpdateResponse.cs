using RpsApi.Models.Enums;

namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class GameUpdateResponse
{
    public GameUpdateAction Action { get; set; }
    public string Message { get; set; } = string.Empty;
}