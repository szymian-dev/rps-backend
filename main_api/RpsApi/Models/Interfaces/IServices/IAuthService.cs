using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;

namespace RpsApi.Models.Interfaces.IServices;

public interface IAuthService
{
    AuthResponse Register(RegisterRequest request, HttpContext httpContext);
    AuthResponse Login(LoginRequest request, HttpContext httpContext);
    AuthResponse RefreshTokens(RefreshRequest request, HttpContext httpContext);
    bool Logout(LogoutRequest request);
    UserResponse GetCurrentUser();
    UserResponse GetUser(int id);
    bool EditUser(UserEditRequest request);  
    DeleteUserResponse DeleteUser();
    PagedResponse<UserResponse> SearchUsers(UserSearchRequest request);
}