using Microsoft.Extensions.Options;
using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Models.Settings;
using RpsApi.Utils;

namespace RpsApi.Services;

public class AuthService(
    IJwtService jwtService,
    IUsersRepository usersRepository,
    IUserContextService userContextService,
    IGameService gameService,
    IGesturesService gesturesService,
    IOptions<CookieSettings> cookieOptions
    ) : IAuthService
{
    private readonly CookieSettings _cookieSettings = ValidateSettings(cookieOptions.Value);
    public AuthResponse Register(RegisterRequest request, HttpContext httpContext)
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

        return CreateSuccessAuthResponse(user, request.DeviceId, httpContext);
    }

    public AuthResponse Login(LoginRequest request, HttpContext httpContext)
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
        return CreateSuccessAuthResponse(user, request.DeviceId, httpContext);
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

    public AuthResponse RefreshTokens(RefreshRequest request, HttpContext httpContext)
    {
        var token = httpContext.Request.GetRefreshTokenFromCookies();
        if (token is null)
        {
            throw new InvalidTokenException("Refresh token not found in HTTP request");
        }
        if(Guid.TryParse(token, out var refreshToken) is false)
        {
            throw new InvalidTokenException("Invalid refresh token");
        }
        var authDto = jwtService.RefreshTokens(request, refreshToken);
        AddRefreshTokenToCookies(authDto.RefreshToken, httpContext);
        return new AuthResponse
        {
            AccessToken = authDto.AccessToken
        };
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
            user.Email = request.NewEmail;
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
            GesturesDeletedMatchRowsChanged = gesturesService.DeleteAllUserGestures(user),
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
    
    private AuthResponse CreateSuccessAuthResponse(User user, Guid deviceId, HttpContext httpContext)
    {
        var token = jwtService.CreateJwtForUser(user);
        var refreshToken = jwtService.CreateOrReplaceRefreshToken(user, deviceId);
        AddRefreshTokenToCookies(refreshToken, httpContext);
        return new AuthResponse
        {
            AccessToken = token,
        };
    }
    
    private void AddRefreshTokenToCookies(RefreshTokenDto refreshTokenDto, HttpContext httpContext)
    {
        httpContext.Response.Cookies.Append("refreshToken", refreshTokenDto.Token.ToString(), new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            SameSite = Enum.Parse<SameSiteMode>(_cookieSettings.SameSite),
            Expires = refreshTokenDto.ExpiresAt
        });
    }
    
    private static CookieSettings ValidateSettings(CookieSettings settings)
    {
        // throw error if value is other than Lax, Strict or None
        if (settings.SameSite != "Lax" && settings.SameSite != "Strict" && settings.SameSite != "None")
        {
            throw new InvalidSettingsException("Invalid SameSite value");
        }
        return settings;
    }
}