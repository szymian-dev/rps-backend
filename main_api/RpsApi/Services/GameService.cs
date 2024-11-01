using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Utils;

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
        if (user.Id == user2.Id)
        {
            throw new InvalidGameException("You cannot play against yourself");
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

    public GameInfoDto GetGameInfo(int gameId)
    {
        var game = gamesRepository.GetGameWithDetails(gameId);
        if (game is null)
        {
            throw new GameNotFoundException("Game not found in the database");
        }
        
        var player1Gesture = game.Gestures.SingleOrDefault(g => g.UserId == game.Player1Id);
        var player2Gesture = game.Gestures.SingleOrDefault(g => g.UserId == game.Player2Id);

        return new GameInfoDto
        {
            GameId = game.Id,
            Status = game.Status,
            WinnerId = game.WinnerId,
            LoserId = game.LoserId,
            IsTie = game.IsTie,
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt,
            Player1 = CreatePlayerResponse(game.Player1, player1Gesture),
            Player2 = CreatePlayerResponse(game.Player2, player2Gesture)
        };
    }

    public PagedResponse<GameInfoDto> GetUsersGames(GamesRequest request)
    {
        var user = userContextService.GetCurrentUser();
        var userGames = gamesRepository.GetGamesWithDetails().Where(g => g.Player1 == user || g.Player2 == user);
        if (!userGames.Any())
        {
            return PaginationHelper.EmptyResponse<GameInfoDto>(request.PageSize);
        }
        var filteredGames = GameFiltersHelper.ApplyGameFilters(userGames, request.Filters, user);
        var calculatedPagination = PaginationHelper.CalculatePagination(filteredGames, request.PageNumber, request.PageSize);
        if (calculatedPagination.TotalCount == 0)
        {
            return PaginationHelper.EmptyResponse<GameInfoDto>(calculatedPagination.PageSize);
        }

        var gamesDtoList = filteredGames
            .ApplyOrdering(request.Ascending, request.SortBy)
            .ApplyPagination(calculatedPagination.PageNumber, calculatedPagination.PageSize)
            .Select(g => new GameInfoDto
            {
                GameId = g.Id,
                Status = g.Status,
                WinnerId = g.WinnerId,
                LoserId = g.LoserId,
                IsTie = g.IsTie,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                Player1 = CreatePlayerResponse(g.Player1, g.Gestures.SingleOrDefault(ge => ge.UserId == g.Player1Id)),
                Player2 = CreatePlayerResponse(g.Player2, g.Gestures.SingleOrDefault(ge => ge.UserId == g.Player2Id))
            })
            .ToList();

        return new PagedResponse<GameInfoDto>
        {
            Items = gamesDtoList,
            PageNumber = calculatedPagination.PageNumber,
            PageSize = calculatedPagination.PageSize,
            TotalCount = calculatedPagination.TotalCount,
            TotalPages = calculatedPagination.TotalPages
        };
    }

    public bool HandleInvitation(HandleInvitationRequest request)
    {
        var game = gamesRepository.GetGame(request.GameId);
        if (game is null)
        {
            throw new GameNotFoundException("Game not found in the database");
        }
        var userId = userContextService.GetCurrentUser().Id;
        if (game.Player2Id != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to accept or decline this invitation");
        }
        var newStatus = request.Accepted ? GameStatus.InProgress : GameStatus.Cancelled;
        if (!game.Status.IsValidTransition(newStatus))
        {
            throw new InvalidGameStatusException($"Cannot join the game. Current status: {game.Status}.");
        }
        game.Status = newStatus;
        return gamesRepository.UpdateGame(game);
    }

    public GameUpdateResponse CheckForGameUpdates(int gameId)
    {
        var game = GetGameInfo(gameId);
        var result = GameUpdateHelper.CheckAndUpdateGame(game);
        
        if (result.Action == GameUpdateAction.NoAction)
        {
            return new GameUpdateResponse
            {
                Action = result.Action,
                Message = result.Message
            };
        }
        
        var gameDb = gamesRepository.GetGame(gameId);
        if (gameDb is null)
        {
            throw new GameNotFoundException("Game not found in the database");
        }
        if (result.Status is not null)
        {
            gameDb.Status = result.Status.Value;
        }         
        gameDb.WinnerId = result.WinnerId;      
        gameDb.LoserId = result.LoserId;         
        gameDb.IsTie = result.IsTie;    
        if(!gamesRepository.UpdateGame(gameDb))
        {
            throw new DatabaseException("Failed to update game state in the database");
        }  
        
        return new GameUpdateResponse
        {
            Action = result.Action,
            Message = result.Message
        };
    }

    public bool CancelAllUserGames(User user)
    {
        var userGames = gamesRepository.GetGames()
           .Where(g => g.Player1Id == user.Id || g.Player2Id == user.Id)
           .Where(g => g.Status == GameStatus.NotStarted || g.Status == GameStatus.InProgress);
        if(!userGames.Any())
        {
           return true;
        }
        return gamesRepository.CancelUserGames(userGames);
    }

    private static PlayerDto CreatePlayerResponse(User? user, Gesture? gesture = null)
    {
        return new PlayerDto
        {
            PlayerInfo = new UserResponse
            {
                Id = user?.Id,
                Username = user?.Username,
                Email = user?.Email,
            },
            SubmittedGesture = gesture is not null ? new GestureInfoDto
            {
                FileId = gesture.Id,
                Type = gesture.GestureType
            } : null
        };
    }
    
}