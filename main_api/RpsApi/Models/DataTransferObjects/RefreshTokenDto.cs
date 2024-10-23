namespace RpsApi.Models.DataTransferObjects;

public class RefreshTokenDto
{
    public required Guid Token { get; set; }
    public required DateTime ExpiresAt { get; set; }
}