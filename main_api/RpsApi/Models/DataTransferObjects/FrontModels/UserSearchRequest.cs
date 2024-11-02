using System.ComponentModel;

namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class UserSearchRequest
{
    [DefaultValue("")] 
    public string SearchTerm { get; set; } = String.Empty;
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
    [DefaultValue("Username")]
    public string SortBy { get; set; } = "Username";
    [DefaultValue(true)]
    public bool Ascending { get; set; } = true;
}