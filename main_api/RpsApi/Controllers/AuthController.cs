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
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    
    public AuthController(IAuthService service)
    {
        _service = service;
    }
    
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
        return _service.Register(request);
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
        return _service.Login(request);
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
    [HttpPost("refresh")]
    public AuthResponse PostRefreshToken(RefreshRequest request)
    {
        return _service.RefreshTokens(request);
    }
}