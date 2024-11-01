namespace RpsApi.Models.Enums;

public enum GameStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public static class GameStatusExtensions
{
    public static bool IsValidTransition(this GameStatus currentStatus, GameStatus newStatus)
    {
        return currentStatus switch
        {
            GameStatus.NotStarted => newStatus == GameStatus.InProgress || newStatus == GameStatus.Cancelled,
            GameStatus.InProgress => newStatus == GameStatus.Completed || newStatus == GameStatus.Cancelled,
            GameStatus.Cancelled => false, 
            GameStatus.Completed => false, 
            _ => false
        };
    }
    
}