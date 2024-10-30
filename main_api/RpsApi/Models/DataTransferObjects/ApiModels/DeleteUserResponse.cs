namespace RpsApi.Models.DataTransferObjects.ApiModels;

public class DeleteUserResponse
{
    public bool GamesCancelledMatchRowsChanged { get; set; }
    public bool RefreshTokensDeletedMatchRowsChanged { get; set; }
    public bool UserDeleted { get; set; }
}