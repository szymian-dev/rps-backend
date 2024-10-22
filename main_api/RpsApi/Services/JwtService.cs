using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;

namespace RpsApi.Services;

public class JwtService(
    IConfiguration configuration,
    IUsersRepository usersRepository,
    IRefreshTokensRepository refreshTokensRepository) : IJwtService
{
    public JwtTokenDto CreateJwToken(User user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        ];
        
        var keySettings = configuration["Jwt:Key"] ?? throw new TokenCreationFailedException("Failed to create token - key missing");
        var issuer = configuration["Jwt:Issuer"] ?? throw new TokenCreationFailedException("Failed to create token - issuer missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keySettings));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        bool succ = int.TryParse(configuration["Jwt:ExpireMinutes"], out int minutes);
        if (!succ)
        {
            throw new TokenCreationFailedException("Failed to create token");
        }

        var token = new JwtSecurityToken(
            issuer: issuer, 
            claims: claims,
            expires: DateTime.Now.AddMinutes(minutes),
            signingCredentials: creds
        );

        return new JwtTokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = token.ValidTo,
            TokenType = "Bearer"
        };
    }

    public RefreshTokenDto CreateOrReplaceRefreshToken(User user, Guid deviceId)
    {
        var expirationDays = configuration.GetSection("Jwt:RefreshTokenExpireDays").Value;
        bool succ = int.TryParse(expirationDays, out int days);
        if (expirationDays is null || !succ)
        {
            throw new TokenCreationFailedException("Failed to create token");
        }
        var refreshToken = refreshTokensRepository.GetRefreshToken(user, deviceId);
        if (refreshToken is null)
        {
            return CreateRefreshToken(user, days, deviceId);
        }
        
        var newToken = Guid.NewGuid();
        var newExpiration = DateTime.Now.AddDays(days);
        if(!refreshTokensRepository.ReplaceRefreshToken(refreshToken, newToken, newExpiration))
        {
            throw new TokenCreationFailedException("Failed to create token");
        }
        return new RefreshTokenDto
        {
            Token = newToken,
            ExpiresAt = newExpiration
        };
    }
    
    public User? GetUserFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        return usersRepository.GetUser(int.Parse(userId));
    }
    
    public AuthResponse RefreshTokens(RefreshRequest request)
    {
        if (!ValidateExpiredToken(request.AccessToken))
        {
            throw new InvalidTokenException("Invalid access token");
        }
        var user = GetUserFromToken(request.AccessToken);
        if (user is null)
        {
            throw new UserNotFoundException("User not found");
        }
        var newRefreshToken = ValidateRefreshToken(request.RefreshToken, user, request.DeviceId);
        var newToken = CreateJwToken(user);
        return new AuthResponse
        {
            AccessToken = newToken,
            RefreshToken = newRefreshToken
        };
    }

    public bool RevokeRefreshToken(User user, Guid deviceId)
    {
        var token = refreshTokensRepository.GetRefreshToken(user, deviceId);
        if (token is null)
        {
            return false;
        }
        return refreshTokensRepository.DeleteRefreshToken(token);
    }
    
    public bool RevokeAllRefreshTokens(User user)
    {
        return refreshTokensRepository.DeleteAllRefreshTokens(user);   
    }
    
    private bool ValidateExpiredToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new Exception("Key not found"))),
            ValidIssuer = configuration["Jwt:Issuer"] ?? throw new Exception("Issuer not found"),
            RequireExpirationTime = false
        }, out SecurityToken validatedToken);
        return validatedToken != null;
    }
    
    
    private RefreshTokenDto CreateRefreshToken(User user, int days, Guid deviceId)
    {
        var token = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid(),
            ExpiresAt = DateTime.Now.AddDays(days),
            DeviceId = deviceId
        };
        refreshTokensRepository.AddRefreshToken(token);
        return new RefreshTokenDto
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt
        };
    }
    
    private RefreshTokenDto ValidateRefreshToken(Guid token, User user, Guid deviceId)
    {
        var refreshToken = refreshTokensRepository.GetRefreshToken(user, deviceId);
        if(refreshToken is null || token != refreshToken.Token || refreshToken.ExpiresAt < DateTime.Now)
        {
            throw new InvalidTokenException("Invalid refresh token");
        }
        bool succ = int.TryParse(configuration.GetSection("Jwt:RefreshTokenExpireDays").Value, out int days);
        if (!succ)
        {
            throw new TokenCreationFailedException("Failed to retrieve token expiration days");
        }
        var newToken = Guid.NewGuid();
        var newExpiration = DateTime.Now.AddDays(days);
        if(!refreshTokensRepository.ReplaceRefreshToken(refreshToken, newToken, newExpiration))
        {
            throw new TokenCreationFailedException("Failed to replace token");
        }
        return new RefreshTokenDto
        {
            Token = newToken,
            ExpiresAt = newExpiration
        };
    }
}