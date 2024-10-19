namespace RpsApi.Models.DataTransferObjects;

public class RefreshTokenDto
{
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
}