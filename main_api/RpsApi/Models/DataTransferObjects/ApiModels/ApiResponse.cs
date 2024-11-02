namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
}