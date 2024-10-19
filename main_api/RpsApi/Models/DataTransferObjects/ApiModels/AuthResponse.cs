namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class AuthResponse
{
    public JwtTokenDto AccessToken { get; set; } = null!;
    public RefreshTokenDto RefreshToken { get; set; } = null!;
}