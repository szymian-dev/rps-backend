using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RpsApi.Models.Database;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.DataTransferObjects.FrontModels;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IRepositories;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Models.Interfaces.ISettings;
using RpsApi.Models.Settings;

namespace RpsApi.Services;

public class JwtService(
    IOptions<JwtSettings> options,
    IOptions<JwtAiModelApiSettings> aiModelOptions,
    IUserContextService userContextService,
    IRefreshTokensRepository refreshTokensRepository,
    IApiCacheService cacheService) : IJwtService
{
    private readonly JwtSettings _settings = ValidateSettings(options.Value);
    private readonly JwtAiModelApiSettings _aiModelSettings = ValidateSettings(aiModelOptions.Value);
    public JwtTokenDto CreateJwtForUser(User user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        ];

        return GenerateJwtToken(claims, _settings);
    }

    public RefreshTokenDto CreateOrReplaceRefreshToken(User user, Guid deviceId)
    {
        var refreshToken = refreshTokensRepository.GetRefreshToken(user, deviceId);
        if (refreshToken is null)
        {
            return CreateRefreshToken(user, _settings.RefreshTokenExpireDays, deviceId);
        }
        
        var newToken = Guid.NewGuid();
        var newExpiration = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpireDays);
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
    
    public AuthDto RefreshTokens(RefreshRequest request, Guid refreshToken)
    {
        if (!ValidateExpiredToken(request.AccessToken))
        {
            throw new InvalidTokenException("Invalid access token");
        }
        var user = userContextService.GetUserFromToken(request.AccessToken);
        if (user is null)
        {
            throw new UserNotFoundException("User not found");
        }
        var newRefreshToken = ValidateRefreshToken(refreshToken, user, request.DeviceId);
        var newToken = CreateJwtForUser(user);
        return new AuthDto
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

    public JwtTokenDto GetJwtForAiModelApi()
    {
        var token = cacheService.RetrieveToken();
        if(token is null || token.ExpiresAt < DateTime.UtcNow)
        {
            cacheService.RetrieveToken();
            return CreateJwtForAiModelApi();
        }
        return token;
    }

    public JwtTokenDto CreateJwtForAiModelApi()
    {
        var claims = new List<Claim>
        {
            new Claim("Client", "AiModelApiClient")
        };
        var token = GenerateJwtToken(claims, _aiModelSettings);
        cacheService.SaveToken(token);
        return token;
    }

    private JwtTokenDto GenerateJwtToken(IEnumerable<Claim> claims, IJwtBaseSettings settings)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(settings.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtTokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = token.ValidTo,
            TokenType = "Bearer"
        };
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
            ValidIssuer = _settings.Issuer,
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
            ExpiresAt = DateTime.UtcNow.AddDays(days),
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
        if(refreshToken is null || token != refreshToken.Token || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidTokenException("Invalid refresh token");
        }
        var newToken = Guid.NewGuid();
        var newExpiration = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpireDays);
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

    private static T ValidateSettings<T>(T settings) where T : IJwtBaseSettings
    {
        if (String.IsNullOrEmpty(settings.Key) || String.IsNullOrEmpty(settings.Issuer) || settings.ExpireMinutes == 0)
        {
            throw new InvalidJwtSettingsException("Invalid or missing JWT settings");
        }
        if (settings is JwtSettings jwtSettings && jwtSettings.RefreshTokenExpireDays == 0)
        {
            throw new InvalidJwtSettingsException("Invalid or missing RefreshTokenExpireDays in JWT settings");
        }
        return settings;
    }
}