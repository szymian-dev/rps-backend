namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class UserResponse
{
    public required int Id { get; set; } 
    public required string Username { get; set; }
    public required string Email { get; set; }
}