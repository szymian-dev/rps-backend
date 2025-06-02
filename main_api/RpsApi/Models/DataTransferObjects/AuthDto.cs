namespace RpsApi.Models.DataTransferObjects;

public class AuthDto
{
    public required JwtTokenDto AccessToken { get; set; } 
    public required RefreshTokenDto RefreshToken { get; set; }
}