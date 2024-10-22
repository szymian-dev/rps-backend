namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class AuthResponse
{
    public required JwtTokenDto AccessToken { get; set; } 
    public required RefreshTokenDto RefreshToken { get; set; } 
}