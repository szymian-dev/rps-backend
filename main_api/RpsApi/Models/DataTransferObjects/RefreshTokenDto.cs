namespace RpsApi.Models.DataTransferObjects;

public class RefreshTokenDto
{
    public Guid Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}