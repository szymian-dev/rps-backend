using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;

namespace RpsApi.Models.Interfaces.IServices;

public interface IGameService
{
    NewGameResponse StartNewGame(NewGameRequest request);
    GameInfoDto GetGameInfo(int gameId);
    PagedResponse<GameInfoDto> GetUsersGames(GamesRequest request);
    bool HandleInvitation(HandleInvitationRequest request);
    bool CheckForGameUpdates(int gameId, int lastGestureId);
    bool CancelAllUserGames(User user);
}