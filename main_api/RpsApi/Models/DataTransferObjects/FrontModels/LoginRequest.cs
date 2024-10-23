namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required Guid DeviceId { get; set; }
}