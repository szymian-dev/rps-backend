using RpsApi.Models.Database;

namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class UserResponse
{
    public int? Id { get; set; } 
    public string? Username { get; set; }
    public string? Email { get; set; }
}