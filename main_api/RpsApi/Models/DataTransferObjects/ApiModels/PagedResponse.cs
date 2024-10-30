
namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
