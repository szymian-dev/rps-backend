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
    
    /// <summary>
    ///    Get currently logged-in user data.
    /// </summary>
    /// <returns>
    ///     Response with user data.
    /// </returns>
    /// <response code="200"> User data retrieved successfully. </response>
    /// <response code="401"> User is not logged in. </response>
    [Authorize]
    [HttpGet("me")]
    public ApiResponse<UserResponse> GetMe()
    {
        return new ApiResponse<UserResponse>()
        {
            Data = service.GetCurrentUser(),
            Message = "Returns user data."
        };
    }
    
    /// <summary>
    ///     Get user data by id.
    /// </summary>
    /// <param name="id">
    ///     ID of the user to get data from.
    /// </param>
    /// <returns>
    ///     Response with user data.
    /// </returns>
    /// <response code="200"> User data retrieved successfully. </response>
    /// <response code="401"> User is not logged in. </response>
    /// <response code="404"> User not found. </response>
    [Authorize]
    [HttpGet("{id}")]
    public ApiResponse<UserResponse> GetUser(int id)
    {
        return new ApiResponse<UserResponse>()
        {
            Data = service.GetUser(id),
            Message = $"Returns user data for id: {id}"
        };        
    }
    
    /// <summary>
    ///     Edits currently logged-in user data.
    /// </summary>
    /// <param name="request">
    ///    Request with new user data.
    /// </param>
    /// <returns>
    ///     True if the user data was edited successfully.
    /// </returns>
    /// <response code="200"> User data edited successfully. </response>
    /// <response code="400"> User with that username or email already exists. Invalid password. </response>
    /// <response code="401"> User is not logged in. </response>
    [Authorize]
    [HttpPut("me")]
    public ApiResponse<bool> PutMe(UserEditRequest request)
    {
        return new ApiResponse<bool>()
        {
            Data = service.EditUser(request),
            Message = "Returns true if the user data was edited successfully."
        };
    }
    
    /// <summary>
    ///     Deletes currently logged-in user. Revokes all refresh tokens.
    /// </summary>
    /// <returns>
    ///    Response with a message if the user was deleted successfully.
    /// </returns>
    /// <response code="200"> User deleted successfully. </response>
    /// <response code="401"> User is not logged in. </response>
    [Authorize]
    [HttpDelete("me")]
    public ApiResponse<DeleteUserResponse> DeleteMe()
    {
        return new ApiResponse<DeleteUserResponse>()
        {
            Data = service.DeleteUser(),
            Message = "Returns information about user removal from the database with details regarding user related tables"
        };
    }
    
    /// <summary>
    ///     Searches for users by username or email.
    /// </summary>
    /// <param name="request">
    ///     Request with search query. 
    /// </param>
    /// <returns>
    ///     Response with found users.
    /// </returns>
    /// <response code="200"> Users found successfully. </response>
    /// <response code="401"> User is not logged in. </response>
    [Authorize]
    [HttpGet("search")]
    public ApiResponse<PagedResponse<UserResponse>> GetSearch([FromQuery] UserSearchRequest request)
    {
        return new ApiResponse<PagedResponse<UserResponse>>()
        {
            Data = service.SearchUsers(request),
            Message = "Returns a list of users that match the search query."
        };
    }
}