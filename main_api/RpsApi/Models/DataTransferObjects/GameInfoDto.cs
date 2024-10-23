using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.Enums;

namespace RpsApi.Models.DataTransferObjects;

public class GameInfoDto
{
    public required int GameId { get; set; }
    public required Player Player1 { get; set; }
    public required Player Player2 { get; set; }
    public required GameStatus Status { get; set; }
    public int? WinnerId { get; set; }
    public int? LoserId { get; set; }
    public bool? IsTie { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}