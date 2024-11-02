using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;

namespace RpsApi.Utils;

public static class GameFiltersHelper
{
    public static IQueryable<Game> ApplyGameFilters(IQueryable<Game> query, GameFilters? filters, User user)
    {
        if (filters == null)
        {
            return query;
        }
        
        if (filters.GameStatus.HasValue)
        {
            query = query.Where(g => g.Status == filters.GameStatus);
        }

        if (filters.OpponentId.HasValue)
        {
            if (filters.OpponentId == user.Id)
            {
                throw new InvalidFilterStateException("User cannot be their own opponent");
            }
            query = query.Where(g => g.Player1Id == filters.OpponentId || g.Player2Id == filters.OpponentId);
        }

        if (filters.PlaysAs.HasValue)
        {
            query = query.Where(g => 
                (g.Player1Id == user.Id && filters.PlaysAs == PlayerEnum.Player1) ||
                (g.Player2Id == user.Id && filters.PlaysAs == PlayerEnum.Player2)
            );
        }

        if (filters.WinStatus.HasValue)
        {
            switch (filters.WinStatus)
            {
                case WinStatus.Won:
                    query = query.Where(g => g.WinnerId == user.Id);
                    break;
                case WinStatus.Lost:
                    query = query.Where(g => g.LoserId == user.Id);
                    break;
                case WinStatus.Draw:
                    query = query.Where(g => g.IsTie == true);
                    break;
                case WinStatus.NotDecided:
                    query = query.Where(g => g.WinnerId == null && g.LoserId == null && g.IsTie == null);
                    break;
            }
        }

        if (filters.DateFrom.HasValue)
        {
            query = query.Where(g => g.CreatedAt >= filters.DateFrom);
        }

        if (filters.DateTo.HasValue)
        {
            query = query.Where(g => g.CreatedAt <= filters.DateTo);
        }

        return query;
    }
}