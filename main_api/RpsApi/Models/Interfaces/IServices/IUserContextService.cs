using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IServices;

public interface IUserContextService
{
    User GetCurrentUser();
    int GetCurrentUserId();
    User? GetUserFromToken(string token);
}