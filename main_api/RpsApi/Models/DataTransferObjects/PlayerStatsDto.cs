using RpsApi.Models.DataTransferObjects.ApiModels;

namespace RpsApi.Models.DataTransferObjects;

public class PlayerStatsDto
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Ties { get; set; }
    public int GamesPlayed { get; set; }
    public required UserResponse PlayerInfo { get; set; }
}