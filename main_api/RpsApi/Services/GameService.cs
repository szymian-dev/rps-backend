using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Services;

public class GameService(IUserContextService userContextService,
    IGamesRepository gamesRepository, IGesturesRepository gesturesRepository, IUsersRepository usersRepository) : IGameService
{
    public NewGameResponse StartNewGame(NewGameRequest request)
    {
        var user = userContextService.GetCurrentUser();
        var user2 = usersRepository.GetUser(request.ChallengedPlayerId);
        if (user2 is null)
        {
            throw new UserNotFoundException("Opponent not found in the database");
        }
        var game = new Game
        {
            Player1Id = user.Id,
            Player2Id = user2.Id,
            Status = GameStatus.NotStarted
        };

        if(!gamesRepository.AddGame(game))
        {
            throw new GameNotCreatedException("Error creating game");
        }

        return new NewGameResponse
        {
            NewGameInfo = new()
            {
                GameId = game.Id,
                Status = game.Status,
                WinnerId = null,
                LoserId = null,
                IsTie = null,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                Player1 = CreatePlayerResponse(user),
                Player2 = CreatePlayerResponse(user2)
            }
        };
    }
    
    private Player CreatePlayerResponse(User user, Gesture? gesture = null)
    {
        return new Player
        {
            PlayerInfo = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            },
            SubmittedGesture = gesture is not null ? new GestureInfoDto
            {
                FileId = gesture.Id,
                Type = gesture.GestureType
            } : null
        };
    }
}