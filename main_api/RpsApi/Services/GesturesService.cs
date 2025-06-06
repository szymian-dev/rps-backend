﻿using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Services;

public class GesturesService(IFileManagementService fileManagementService, IAiModelApiService aiModelApiService, 
    IGesturesRepository gesturesRepository, IGameService gameService, IUserContextService userContextService) : IGesturesService
{
    public async Task<GameUpdateResponse> UploadGesture(IFormFile file, int modelId, int gameId)
    {
        var game = gameService.GetGameInfo(gameId);
        var user = userContextService.GetCurrentUser();
        ValidateGestureAddition(game, user);
        
        string fileName = fileManagementService.UploadFile(file);
        string filePath = Path.Combine(fileManagementService.GetUploadDirectoryPath(), fileName);
        
        GestureType? gestureType;
        try
        {
            gestureType = await aiModelApiService.AnalyzeGesture(filePath, modelId);
        }
        catch (Exception e)
        {
            fileManagementService.DeleteFile(fileName);
            throw;
        }
        if(gestureType is null)
        {
            fileManagementService.DeleteFile(fileName);
            throw new UnprocessableEntityException("The model was unable to analyze the gesture, please try again");
        }
        
        var newGesture = new Gesture
        {
            GestureType = gestureType,
            FilePath = fileName,
            GameId = gameId,
            UserId = user.Id
        };

        bool succ;
        try
        {
            succ = gesturesRepository.AddGesture(newGesture);
        }
        catch (Exception e)
        {
            fileManagementService.DeleteFile(fileName);
            throw;
        }
        if(!succ)
        {
            fileManagementService.DeleteFile(fileName);
            throw new DatabaseException("Failed to add gesture to database");
        }

        GameUpdateResponse response;
        try
        {
            response = gameService.CheckForGameUpdates(gameId);
        }
        catch (Exception e)
        {
            fileManagementService.DeleteFile(fileName);
            gesturesRepository.DeleteGesture(newGesture);
            throw;
        }
        return response;
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
            throw new ForbiddenAccessException("User is not a player in this game");
        }

        if (game.Status != GameStatus.Completed && gesture.UserId != user.Id)
        {
            throw new ForbiddenAccessException("Game is not yet completed");
        }

        FileStreamResult fileStream;
        try
        {
            fileStream = fileManagementService.GetFile(gesture.FilePath);
        }
        catch (FileNotFoundException e)
        {
            var exc = new InvalidGameStateException("Gesture file not found, but record in the database. Removing gesture from database", e);
            var result = gesturesRepository.DeleteGesture(gesture);
            if (!result)
            {
                throw new DatabaseException("Failed to remove gesture from database", exc);
            }
            throw exc;
        }
        return fileStream;
    }
    
    public async Task<List<AiModelDto>> GetAiModels()
    {
        return await aiModelApiService.GetAiModels();
    }
    
    public async Task<bool> GiveFeedback(int modelId, bool wrongPrediction)
    {
        return await aiModelApiService.GiveFeedback(modelId, wrongPrediction);
    }

    public bool DeleteAllUserGestures(User user)
    {
        var gestures = gesturesRepository.GetAllUserGestures(user.Id).ToList();
        if (!gestures.Any())
        {
            return true;
        }
        bool deletedSuccessfully = true;
        foreach (var gesture in gestures)
        {
            if(gesturesRepository.DeleteGesture(gesture) == false || fileManagementService.DeleteFile(gesture.FilePath) == false)
            {
                deletedSuccessfully = false;
            }
        }
        return deletedSuccessfully;
    }

    private static void ValidateGestureAddition(GameInfoDto game, User user)
    {
        if(game.Player1.PlayerInfo.Id is null || game.Player2.PlayerInfo.Id is null)
        {
            throw new InvalidGameStateException("Game is cancelled as one of the players has been deleted");
        }
        if(game.Player1.PlayerInfo.Id != user.Id && game.Player2.PlayerInfo.Id != user.Id)
        {
            throw new ForbiddenAccessException("User is not a player in this game");
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