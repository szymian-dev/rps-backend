using RpsApi.Models.DataTransferObjects.ApiModels;

namespace RpsApi.Models.DataTransferObjects;

public class Player
{
    public required UserResponse PlayerInfo { get; set; }
    public GestureInfoDto? SubmittedGesture { get; set; }
}