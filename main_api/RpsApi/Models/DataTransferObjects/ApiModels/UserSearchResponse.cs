namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class UserSearchResponse
{
    public List<UserResponse> Users { get; set; } = new List<UserResponse>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }   
}