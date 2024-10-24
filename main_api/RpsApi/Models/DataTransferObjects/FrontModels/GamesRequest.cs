namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class GamesRequest
{
    public GameFilters? Filters { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "Id";
    public bool Ascending { get; set; } = true;
}