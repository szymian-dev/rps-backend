namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class UserEditRequest
{
    public string? NewUsername { get; set; }
    public string? NewEmail { get; set; }
    public string? NewPassword { get; set; }
    public string? CurrentPassword { get; set; }
}