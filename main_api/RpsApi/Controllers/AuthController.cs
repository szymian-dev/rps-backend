using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Controllers;

/// <summary>
///    Controller for authentication and authorization of users.
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController(IAuthService service) : ControllerBase
{
    /// <summary>
    ///  Register new user.
    /// </summary>
    /// <param name="request">
    ///     Request with user data.
    /// </param>
    /// <returns>
    ///     Response with an access token and a refresh token.
    /// </returns>
    /// <response code="200"> User registered successfully. </response>
    /// <response code="400"> User with that username already exists. </response>
    [HttpPost("register")]
    public ApiResponse<AuthResponse> PostRegister(RegisterRequest request)
    {
        return new ApiResponse<AuthResponse>()
        {
            Data = service.Register(request, HttpContext),
            Message = "User registered successfully. Response contains access token with its expiration time and set-cookie header with refresh token."
        };
    }
    
    /// <summary>
    ///  Login user.
    /// </summary>
    /// <param name="request">
    ///     Request with user data.
    /// </param>
    /// <returns>
    ///     Response with an access token and a refresh token.
    /// </returns>
    /// <response code="200"> User logged in successfully. </response>
    /// <response code="400"> User with that username does not exist or password is incorrect. </response>
    [HttpPost("login")]
    public ApiResponse<AuthResponse> PostLogin(LoginRequest request)
    {
        return new ApiResponse<AuthResponse>()
        {
            Data = service.Login(request, HttpContext),
            Message = "User logged in successfully. Response contains access token with its expiration time and set-cookie header with refresh token."
        };
    }
    
    /// <summary>
    ///  Refresh access token.
    /// </summary>
    /// <param name="request">
    ///    Request with a still valid refresh token and an expired access token.
    /// </param>
    /// <returns>
    ///    Response with a new access token and a new refresh token.
    /// </returns>
    /// <response code="200"> Access token refreshed successfully. </response>
    /// <response code="400"> User with that username does not exist </response>
    /// <response code="401"> Refresh and/or access token is invalid. </response>
    [HttpPost("refresh-token")]
    public ApiResponse<AuthResponse> PostRefreshToken(RefreshRequest request)
    {
        return new ApiResponse<AuthResponse>()
        {
            Data = service.RefreshTokens(request, HttpContext),
            Message = "Access token refreshed successfully. Response contains new access token with its expiration time and set-cookie header with new refresh token."
        };
    }
    
    /// <summary>
    ///     Logs the user out, revokes refresh token from a specific device or all devices if no device id is provided.
    /// </summary>
    /// <param name="request">
    ///     Request with a device id to revoke refresh token from.
    /// </param>
    /// <returns>
    ///     True if the refresh token was revoked successfully.
    /// </returns>
    /// <response code="200"> User logged out successfully. </response>
    /// <response code="400"> User with that username does not exist. </response>
    /// <response code="401"> User is not logged in. </response>
    [Authorize]
    [HttpDelete("logout")]
    public ApiResponse<bool> DeleteLogout(LogoutRequest request)
    {
        return new ApiResponse<bool>()
        {
            Data = service.Logout(request),
            Message = "Returns true if the refresh token was revoked successfully."
        };
    }
}