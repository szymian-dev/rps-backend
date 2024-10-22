using RpsApi.Database;
using RpsApi.Models.Database;
using RpsApi.Models.Interfaces.IRepositories;

namespace RpsApi.Repositories;

public class UsersRepository(ApplicationDbContext dbContext) : IUsersRepository
{
    public User? GetUser(int id)
    {
        return dbContext.Users.SingleOrDefault(u => u.Id == id);
    }
    
    public User? GetUser(string username)
    {
        return dbContext.Users.SingleOrDefault(u => u.Username == username);
    }

    public User? GetUserByEmail(string email)
    {
        return dbContext.Users.SingleOrDefault(u => u.Email == email);
    }

    public User? GetUserByUsernameOrEmail(string usernameOrEmail)
    {
        return dbContext.Users.SingleOrDefault(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public IQueryable<User> GetUsers()
    {
        return dbContext.Users;
    }

    public bool AddUser(User user)
    {
        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;
        dbContext.Users.Add(user);
        int changes = dbContext.SaveChanges();
        return changes > 0;
    }

    public bool UpdateUser(User user)
    {
        user.UpdatedAt = DateTime.Now;
        dbContext.Users.Update(user);
        int changes = dbContext.SaveChanges();
        return changes > 0;
    }

    public bool DeleteUser(User user)
    {
        dbContext.Users.Remove(user);
        int changes = dbContext.SaveChanges();
        return changes > 0;
    }
}