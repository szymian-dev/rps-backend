using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IRepositories;

public interface IGamesRepository
{
    bool AddGame(Game game);
    Game? GetGame(int gameId);
    Game? GetGameWithDetails(int gameId);
    IQueryable<Game> GetGamesWithDetails();
    bool UpdateGame(Game game);
}