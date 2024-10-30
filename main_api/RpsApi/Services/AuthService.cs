using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Utils;

namespace RpsApi.Services;

public class AuthService(
    IJwtService jwtService,
    IUsersRepository usersRepository,
    IUserContextService userContextService,
    IGameService gameService
    ) : IAuthService
{
    public AuthResponse Register(RegisterRequest request)
    {
        if (usersRepository.GetUser(request.Username) is not null )
        {
            throw new UserAlreadyExistsException("User with that username already exists");
        }
        if (usersRepository.GetUserByEmail(request.Email) is not null)
        {
            throw new UserAlreadyExistsException("User with that email already exists");
        }
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            Username = request.Username,
            PasswordHash = hashedPassword,
            Email = request.Email
        };
        
        var success = usersRepository.AddUser(user);
        if(!success)
        {
            throw new UserCreationFailedException("Failed to create user");
        }

        return CreateSuccessAuthResponse(user, request.DeviceId);
    }

    public AuthResponse Login(LoginRequest request)
    {
        var user = usersRepository.GetUserByUsernameOrEmail(request.Username);
        if (user is null)
        {
            throw new UserNotFoundException("User not found");
        }
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidPasswordException("Invalid password");
        }
        return CreateSuccessAuthResponse(user, request.DeviceId);
    }
    
    public bool Logout(LogoutRequest request)
    {
        var user = userContextService.GetCurrentUser();
        if (request.DeviceId is null)
        {
            return jwtService.RevokeAllRefreshTokens(user);
        }
        if(jwtService.RevokeRefreshToken(user, request.DeviceId.Value) is false)
        {
            return jwtService.RevokeAllRefreshTokens(user);
        }

        return true;
    }

    public AuthResponse RefreshTokens(RefreshRequest request)
    {
        return jwtService.RefreshTokens(request);
    }

    public UserResponse GetCurrentUser()
    {
        var user = userContextService.GetCurrentUser();
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public UserResponse GetUser(int id)
    {
        var user = usersRepository.GetUser(id);
        if (user is null)
        {
            throw new UserNotFoundException("User not found");
        }
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public bool EditUser(UserEditRequest request)
    {
        var user = userContextService.GetCurrentUser();
        if(!string.IsNullOrEmpty(request.NewUsername) && request.NewUsername != user.Username)
        {
            if(usersRepository.GetUser(request.NewUsername) is not null)
            {
                throw new UserAlreadyExistsException("User with that username already exists!");
            }
            user.Username = request.NewUsername;
        }
        
        if(!string.IsNullOrEmpty(request.NewEmail) && request.NewEmail != user.Email)
        {
            if(usersRepository.GetUserByEmail(request.NewEmail) is not null)
            {
                throw new UserAlreadyExistsException("User with that email already exists!");
            }
        }

        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            if(!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new InvalidPasswordException("Invalid current password");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        }

        return usersRepository.UpdateUser(user);
    }

    public DeleteUserResponse DeleteUser()
    {
        var user = userContextService.GetCurrentUser();
        return new DeleteUserResponse()
        {
            GamesCancelledMatchRowsChanged = gameService.CancelAllUserGames(user),
            RefreshTokensDeletedMatchRowsChanged = jwtService.RevokeAllRefreshTokens(user),
            UserDeleted = usersRepository.DeleteUser(user)
        };
    }
    
    public PagedResponse<UserResponse> SearchUsers(UserSearchRequest request)
    {
        var filteredUsers = usersRepository.GetUsers()
            .Where(u => u.Username.Contains(request.SearchTerm) || u.Email.Contains(request.SearchTerm));
        var calculatedPagination = PaginationHelper.CalculatePagination(filteredUsers, request.PageNumber, request.PageSize);
        if (calculatedPagination.TotalCount == 0)
        {
            return PaginationHelper.EmptyResponse<UserResponse>(request.PageSize);
        }
        var userList = filteredUsers
            .ApplyOrdering(request.Ascending, request.SortBy)
            .ApplyPagination(calculatedPagination.PageNumber, calculatedPagination.PageSize)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
            })
            .ToList();
        return new PagedResponse<UserResponse>()
        {
            Items = userList,
            PageNumber = calculatedPagination.PageNumber,
            PageSize = calculatedPagination.PageSize,
            TotalCount = calculatedPagination.TotalCount,
            TotalPages = calculatedPagination.TotalPages
        };
    }
    
    private AuthResponse CreateSuccessAuthResponse(User user, Guid deviceId)
    {
        var token = jwtService.CreateJwToken(user);
        var refreshToken = jwtService.CreateOrReplaceRefreshToken(user, deviceId);
        return new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
    }
}