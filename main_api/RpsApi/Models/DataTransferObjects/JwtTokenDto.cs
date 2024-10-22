namespace RpsApi.Models.DataTransferObjects;

public class JwtTokenDto
{
    public string Token { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
}