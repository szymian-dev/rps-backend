using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Services;

public class GestureService(IFileManagementService fileManagementService, IAiModelApiService aiModelApiService, 
    IGesturesRepository gesturesRepository, IGameService gameService, IUserContextService userContextService) : IGestureService
{
    public async Task<GameUpdateResponse> UploadGesture(IFormFile file, int gameId)
    {
        var game = gameService.GetGameInfo(gameId);
        var user = userContextService.GetCurrentUser();
        ValidateGestureAddition(game, user);
        
        string fileName = fileManagementService.UploadFile(file);
        string filePath = Path.Combine(fileManagementService.GetUploadDirectoryPath(), fileName);
        GestureType gestureType = await aiModelApiService.AnalyzeGesture(filePath);
        var newGesture = new Gesture
        {
            GestureType = gestureType,
            FilePath = fileName,
            GameId = gameId,
            UserId = user.Id
        };
        bool succ = gesturesRepository.AddGesture(newGesture);
        if(!succ)
        {
            fileManagementService.DeleteFile(fileName);
            throw new DatabaseException("Failed to add gesture to database");
        }
        return gameService.CheckForGameUpdates(gameId);
    }

    public FileStreamResult GetGesture(int fileId)
    {
        var user = userContextService.GetCurrentUser();
        var gesture = gesturesRepository.GetGesture(fileId);
        if(gesture is null)
        {
            throw new NotFoundException("Gesture not found");
        }
        var game = gameService.GetGameInfo(gesture.GameId);
        if(game.Player1.PlayerInfo.Id != user.Id && game.Player2.PlayerInfo.Id != user.Id)
        {
            throw new UnauthorizedAccessException("User is not a player in this game");
        }
        return fileManagementService.GetFile(gesture.FilePath);
    }

    private static void ValidateGestureAddition(GameInfoDto game, User user)
    {
        if(game.Player1.PlayerInfo.Id is null || game.Player2.PlayerInfo.Id is null)
        {
            throw new InvalidGameStateException("Game is cancelled as one of the players has been deleted");
        }
        if(game.Player1.PlayerInfo.Id != user.Id && game.Player2.PlayerInfo.Id != user.Id)
        {
            throw new UnauthorizedAccessException("User is not a player in this game");
        }
        if(game.Status != GameStatus.InProgress)
        {
            throw new InvalidGameStateException("Game is not in progress");
        }
        if(user.Id == game.Player1.PlayerInfo.Id && game.Player1.SubmittedGesture is not null)
        {
            throw new InvalidGameStateException("Player 1 has already made a gesture");
        }
        if(user.Id == game.Player2.PlayerInfo.Id && game.Player2.SubmittedGesture is not null)
        {
            throw new InvalidGameStateException("Player 2 has already made a gesture");
        }
    }
}