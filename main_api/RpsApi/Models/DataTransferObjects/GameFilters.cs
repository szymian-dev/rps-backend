using RpsApi.Models.Enums;

namespace RpsApi.Models.DataTransferObjects;

public class GameFilters
{
    public GameStatus? GameStatus { get; set; }
    public int? OpponentId { get; set; }
    public PlayerEnum? PlaysAs { get; set; }
    public WinStatus? WinStatus { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

