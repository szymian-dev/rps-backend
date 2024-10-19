using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Services;

public class AuthService : IAuthService
{
    public AuthResponse Register(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public AuthResponse Login(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public AuthResponse RefreshTokens(RefreshRequest request)
    {
        throw new NotImplementedException();
    }
}