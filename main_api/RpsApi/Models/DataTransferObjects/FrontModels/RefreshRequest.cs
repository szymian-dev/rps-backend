namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class RefreshRequest
{
    public required string AccessToken { get; set; }
    public required Guid DeviceId { get; set; }
}