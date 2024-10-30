namespace RpsApi.Models.DataTransferObjects.FrontModels;

public class HandleInvitationRequest
{
    public required int GameId { get; set; }
    public required bool Accepted { get; set; }
}