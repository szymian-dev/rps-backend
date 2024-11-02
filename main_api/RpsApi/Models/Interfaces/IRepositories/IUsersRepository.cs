using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IRepositories;

public interface IUsersRepository
{
    User? GetUser(int id);
    User? GetUser(string username);
    User? GetUserByEmail(string email);
    User? GetUserByUsernameOrEmail(string usernameOrEmail);
    IQueryable<User> GetUsers();
    bool AddUser(User user);
    bool UpdateUser(User user);
    bool DeleteUser(User user);
}