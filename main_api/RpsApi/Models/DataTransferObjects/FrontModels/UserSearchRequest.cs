namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class UserSearchRequest
{
    public string SearchTerm { get; set; } = null!;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "Username";
    public bool Ascending { get; set; } = true;
}