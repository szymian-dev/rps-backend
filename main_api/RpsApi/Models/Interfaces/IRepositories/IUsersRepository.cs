using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IRepositories;

public interface IUsersRepository
{
    User GetUser(int id);
}