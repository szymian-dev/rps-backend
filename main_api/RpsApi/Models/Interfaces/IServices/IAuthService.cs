using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;

namespace RpsApi.Models.Interfaces.IServices;

public interface IAuthService
{
    AuthResponse Register(RegisterRequest request);
    AuthResponse Login(LoginRequest request);
    AuthResponse RefreshTokens(RefreshRequest request);
    bool Logout(LogoutRequest request);
    UserResponse GetUser();
    bool EditUser(UserEditRequest request);  
    bool DeleteUser();
    UserSearchResponse SearchUsers(UserSearchRequest request);
}