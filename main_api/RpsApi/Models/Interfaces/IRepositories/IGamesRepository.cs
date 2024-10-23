using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IRepositories;

public interface IGamesRepository
{
    bool AddGame(Game game);
}