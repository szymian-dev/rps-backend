namespace RpsApi.Models.DataTransferObjects;

public class JwtTokenDto
{
    public required string Token { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public required DateTime ExpiresAt { get; set; }
}