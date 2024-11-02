using System.ComponentModel;

namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class LeaderboardRequest
{
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
}