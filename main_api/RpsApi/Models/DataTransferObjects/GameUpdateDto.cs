using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;

namespace RpsApi.Models.DataTransferObjects;

public class GameUpdateDto
{
    public GameUpdateAction Action { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool? IsTie { get; set; }
    public int? WinnerId { get; set; }
    public int? LoserId { get; set; }
    public GameStatus? Status { get; set; }
}
public static class GameUpdateDtoExtensions
{
    public static void SetStatusIfValid(this GameUpdateDto gameUpdateDto, GameStatus newStatus)
    {
        if (gameUpdateDto.Status is null || !gameUpdateDto.Status.Value.IsValidTransition(newStatus))
        {
            throw new InvalidGameStatusException($"Game is not in a valid state to transition to {newStatus}. Current status: {gameUpdateDto.Status}");
        }
        gameUpdateDto.Status = newStatus;
    }
    public static void SetStatusIfValid(this GameUpdateDto gameUpdateDto, GameStatus oldStatus, GameStatus newStatus)
    {
        if (!oldStatus.IsValidTransition(newStatus))
        {
            throw new InvalidGameStatusException($"Game is not in a valid state to transition to {newStatus}. Current status: {oldStatus}");
        }
        gameUpdateDto.Status = newStatus;
    }
}
