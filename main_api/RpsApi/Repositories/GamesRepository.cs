using Microsoft.EntityFrameworkCore;
using RpsApi.Database;
using RpsApi.Models.Database;
using RpsApi.Models.Enums;
using RpsApi.Models.Interfaces.IRepositories;

namespace RpsApi.Repositories;

public class GamesRepository(ApplicationDbContext dbContext) : IGamesRepository
{
    public bool AddGame(Game game)
    {
        game.CreatedAt = DateTime.UtcNow;
        game.UpdatedAt = DateTime.UtcNow;
        dbContext.Games.Add(game);
        return dbContext.SaveChanges() > 0;
    }
    public Game? GetGame(int gameId)
    {
        return dbContext.Games.SingleOrDefault(g => g.Id == gameId);
    }
    public Game? GetGameWithDetails(int gameId)
    {
        return dbContext.Games
            .Include(g => g.Player1)
            .Include(g => g.Player2)
            .Include(g => g.Gestures) 
            .SingleOrDefault(g => g.Id == gameId);
    }
    
    public IQueryable<Game> GetGamesWithDetails()
    {
        return dbContext.Games
            .Include(g => g.Player1)
            .Include(g => g.Player2)
            .Include(g => g.Gestures);
    } 
    
    public IQueryable<Game> GetGames()
    {
        return dbContext.Games;
    }
    
    public bool UpdateGame(Game game)
    {
        game.UpdatedAt = DateTime.UtcNow;
        dbContext.Games.Update(game);
        return dbContext.SaveChanges() > 0;
    }

    public bool CancelUserGames(IQueryable<Game> games)
    {
        int records = games.Count();
        int changes = games.ExecuteUpdate(s => s.SetProperty(g => g.Status, GameStatus.Cancelled));
        return records == changes;
    }
}