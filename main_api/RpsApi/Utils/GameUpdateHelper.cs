using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;

namespace RpsApi.Utils;

public static class GameUpdateHelper
{
    public static GameUpdateDto CheckAndUpdateGame(GameInfoDto game)
    {
        return game.Status switch
        {
            GameStatus.Cancelled => CancelledGameStatusResponse(game),
            GameStatus.InProgress => EvaluateGestures(game),
            GameStatus.NotStarted => throw new InvalidGameStateException("Game is not in progress! Cannot update or upload gestures!"),
            GameStatus.Completed => throw new InvalidGameStateException("Game is already completed! Cannot update or upload gestures!"),
            _ => CancelGameStatusResponse(game)
        };
    }

    private static GameUpdateDto EvaluateGestures(GameInfoDto game)
    {
        if (game.Player1.PlayerInfo.Id is null || game.Player2.PlayerInfo.Id is null)
        {
            return CancelGameStatusResponse(game);
        }
        if (game.Player1.SubmittedGesture is null || game.Player2.SubmittedGesture is null)
        {
            return WaitingForOpponentResponse(game);
        }
        return CalculateWinner(game);
    }

    private static GameUpdateDto CancelledGameStatusResponse(GameInfoDto game)
    {
        string message = game.Player1.PlayerInfo.Id is null || game.Player2.PlayerInfo.Id is null
            ? "Game is cancelled due to a missing player."
            : "Game status: Cancelled";

        return new GameUpdateDto
        {
            Action = GameUpdateAction.NoAction,
            Message = message
        };
    }

    private static GameUpdateDto CancelGameStatusResponse(GameInfoDto game)
    {
        var response = new GameUpdateDto
        {
            Action = GameUpdateAction.Cancel,
            Message = "Game is cancelled as one of the players has been deleted",
        };
        response.SetStatusIfValid(game.Status, GameStatus.Cancelled);
        return response;
    }

    private static GameUpdateDto WaitingForOpponentResponse(GameInfoDto game)
    {
        var message = game.Player1.SubmittedGesture is null && game.Player2.SubmittedGesture is not null
            ? "Waiting for Player 1 to submit the gesture"
            : game.Player2.SubmittedGesture is null && game.Player1.SubmittedGesture is not null
                ? "Waiting for Player 2 to submit the gesture"
                : "Waiting for both players to submit gestures";

        return new GameUpdateDto
        {
            Action = GameUpdateAction.NoAction,
            Message = message
        };
    }

    private static GameUpdateDto CalculateWinner(GameInfoDto game)
    {
        var player1Gesture = game.Player1.SubmittedGesture?.Type ?? throw new InvalidGameStateException("Player 1 gesture is missing");
        var player2Gesture = game.Player2.SubmittedGesture?.Type ?? throw new InvalidGameStateException("Player 2 gesture is missing");

        var result = DetermineWinner(player1Gesture, player2Gesture);
        var gameStatusResponse = new GameUpdateDto
        {
            Action = result == 0 ? GameUpdateAction.Draw : result == 1 ? GameUpdateAction.Player1Wins : GameUpdateAction.Player2Wins,
            Message = GetWinnerMessage(result, player1Gesture, player2Gesture),
            IsTie = result == 0,
            WinnerId = result == 1 ? game.Player1.PlayerInfo.Id : result == -1 ? game.Player2.PlayerInfo.Id : null,
            LoserId = result == 1 ? game.Player2.PlayerInfo.Id : result == -1 ? game.Player1.PlayerInfo.Id : null
        };

        gameStatusResponse.SetStatusIfValid(game.Status, GameStatus.Completed);
        return gameStatusResponse;
    }

    private static int DetermineWinner(GestureType player1Gesture, GestureType player2Gesture)
    {
        if (player1Gesture == player2Gesture) return 0;

        return (player1Gesture, player2Gesture) switch
        {
            (GestureType.Rock, GestureType.Scissors) => 1,
            (GestureType.Scissors, GestureType.Paper) => 1,
            (GestureType.Paper, GestureType.Rock) => 1,
            _ => -1
        };
    }

    private static string GetWinnerMessage(int result, GestureType player1Gesture, GestureType player2Gesture)
    {
        return result switch
        {
            0 => $"It's a tie! Both players submitted the same gesture: {player1Gesture}",
            1 => $"Player 1 wins! {player1Gesture} beats {player2Gesture}",
            -1 => $"Player 2 wins! {player2Gesture} beats {player1Gesture}",
            _ => throw new InvalidGameStateException("Invalid game result")
        };
    }
}