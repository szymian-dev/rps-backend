using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RpsApi.Models.Database;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Utils;

namespace RpsApi.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor, IUsersRepository usersRepository)
    : IUserContextService
{
    public User GetCurrentUser()
    {
        int userId = GetCurrentUserId();
        return usersRepository.GetUser(userId) ?? throw new UserNotFoundException("User not found");
    }

    public int GetCurrentUserId()
    {
        var token = httpContextAccessor.GetJwtToken();
        if (string.IsNullOrEmpty(token))
        {
            throw new JwtTokenAccessException("Error accessing jwt token from HTTP context");
        }
        int userId = GetUserIdFromToken(token) ?? throw new JwtTokenAccessException("Error accessing user id from jwt token");
        return userId;
    }

    public User? GetUserFromToken(string token)
    {
        var userId = GetUserIdFromToken(token);
        return userId is null ? null : usersRepository.GetUser(userId.Value);
    }
    
    private int? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userIdString = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        if(!int.TryParse(userIdString, out var userId))
        {
            return null;
        }

        return userId;
    }
}
    