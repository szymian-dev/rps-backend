namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class RefreshRequest
{
    public string AccessToken { get; set; }
    public Guid RefreshToken { get; set; }
}