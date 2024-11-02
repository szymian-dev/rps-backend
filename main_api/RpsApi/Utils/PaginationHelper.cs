using RpsApi.Models.DataTransferObjects.ApiModels;

namespace RpsApi.Utils;

public static class PaginationHelper
{
    public record CalculatedPagination(int PageNumber, int PageSize, int TotalCount, int TotalPages);
    
    public static CalculatedPagination CalculatePagination<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        pageSize = Math.Clamp(pageSize, 1, 1000);
        
        int totalCount = query.Count();
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (totalPages == 0)
        {
            totalPages = 1;
        }
        
        pageNumber = Math.Clamp(pageNumber, 1, totalPages);

        return new CalculatedPagination(pageNumber, pageSize, totalCount, totalPages);
    }
    
    public static PagedResponse<T> EmptyResponse<T>(int pageSize)
    {
        pageSize = Math.Clamp(pageSize, 1, 1000);
        return new PagedResponse<T>
        {
            Items = new List<T>(),
            PageNumber = 1,
            PageSize = pageSize,
            TotalCount = 0,
            TotalPages = 1
        };
    }
}