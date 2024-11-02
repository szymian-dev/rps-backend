using RpsApi.Models.Enums;

namespace RpsApi.Models.DataTransferObjects;

public class StatsUpdate
{
    public int UserId { get; set; }
    public WinStatus WinStatus { get; set; }
}