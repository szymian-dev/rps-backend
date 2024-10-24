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
    public AuthResponse PostRegister(RegisterRequest request)
    {
        return service.Register(request);
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
    public AuthResponse PostLogin(LoginRequest request)
    {
        return service.Login(request);
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
    public AuthResponse PostRefreshToken(RefreshRequest request)
    {
        return service.RefreshTokens(request);
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
    /// <response code="401"> User is not logged in. </response>
    /// <response code="400"> User with that username does not exist. </response>
    [Authorize]
    [HttpDelete("logout")]
    public bool DeleteLogout(LogoutRequest request)
    {
        return service.Logout(request);
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
    public UserResponse GetMe()
    {
        return service.GetCurrentUser();
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
    public UserResponse GetUser(int id)
    {
        return service.GetUser(id);        
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
    /// <response code="400"> User with that username or email already exists. </response>
    /// <response code="400"> Invalid password. </response>
    /// <response code="401"> User is not logged in. </response>
    [Authorize]
    [HttpPut("me")]
    public bool PutMe(UserEditRequest request)
    {
        return service.EditUser(request);
    }
    
    /// <summary>
    ///     Deletes currently logged-in user. Revokes all refresh tokens.
    /// </summary>
    /// <returns>
    ///    True if the user was deleted successfully.
    /// </returns>
    /// <response code="200"> User deleted successfully. </response>
    /// <response code="401"> User is not logged in. </response>
    [Authorize]
    [HttpDelete("me")]
    public bool DeleteMe()
    {
        return service.DeleteUser();
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
    public UserSearchResponse GetSearch(UserSearchRequest request)
    {
        return service.SearchUsers(request);
    }
}