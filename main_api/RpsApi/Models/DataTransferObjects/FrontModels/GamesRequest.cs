using System.ComponentModel;

namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class GamesRequest
{
    public GameFilters? Filters { get; set; }
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
    [DefaultValue("Id")]
    public string SortBy { get; set; } = "Id";
    [DefaultValue(true)]
    public bool Ascending { get; set; } = true;
}