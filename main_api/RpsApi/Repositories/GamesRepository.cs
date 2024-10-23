using RpsApi.Database;
using RpsApi.Models.Database;
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
}