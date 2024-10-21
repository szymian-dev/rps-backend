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

public class JwtService
{
    private readonly IConfiguration _configuration;
    private readonly IUsersRepository _usersRepository;
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    
    public JwtService(IConfiguration configuration, IUsersRepository usersRepository, IRefreshTokensRepository refreshTokensRepository)
    {
        _configuration = configuration;
        _usersRepository = usersRepository;
        _refreshTokensRepository = refreshTokensRepository;
    }

    public JwtTokenDto CreateJwToken(User user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        ];
        
        var keySettings = _configuration["Jwt:Key"] ?? throw new TokenCreationFailedException("Failed to create token - key missing");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new TokenCreationFailedException("Failed to create token - issuer missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keySettings));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        bool succ = int.TryParse(_configuration["Jwt:ExpireMinutes"], out int minutes);
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
            Expires = token.ValidTo,
            TokenType = "Bearer"
        };
    }

    public RefreshTokenDto CreateOrReplaceRefreshToken(User user)
    {
        var expirationDays = _configuration.GetSection("Jwt:RefreshTokenExpireDays").Value;
        bool succ = int.TryParse(expirationDays, out int days);
        if (expirationDays is null || !succ)
        {
            throw new TokenCreationFailedException("Failed to create token");
        }
        var refreshToken = _refreshTokensRepository.GetRefreshToken(user);
        if (refreshToken is null)
        {
            return CreateRefreshToken(user, days);
        }
        
        var newToken = Guid.NewGuid();
        var newExpiration = DateTime.Now.AddDays(days);
        if(!_refreshTokensRepository.ReplaceRefreshToken(refreshToken, newToken, newExpiration))
        {
            throw new TokenCreationFailedException("Failed to create token");
        }
        return new RefreshTokenDto
        {
            Token = newToken.ToString(),
            Expires = newExpiration
        };
    }
    
    public User GetUserFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        return _usersRepository.GetUser(int.Parse(userId));
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
        var newRefreshToken = ValidateRefreshToken(request.RefreshToken, user);
        var newToken = CreateJwToken(user);
        return new AuthResponse
        {
            AccessToken = newToken,
            RefreshToken = newRefreshToken
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new Exception("Key not found"))),
            ValidIssuer = _configuration["Jwt:Issuer"] ?? throw new Exception("Issuer not found"),
            RequireExpirationTime = false
        }, out SecurityToken validatedToken);
        return validatedToken != null;
    }
    
    
    private RefreshTokenDto CreateRefreshToken(User user, int days = 30)
    {
        var token = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            Expires = DateTime.Now.AddDays(days)
        };
        _refreshTokensRepository.AddRefreshToken(token);
        return new RefreshTokenDto
        {
            Token = token.Token,
            Expires = token.Expires
        };
    }
    
    private RefreshTokenDto ValidateRefreshToken(string token, User user)
    {
        var refreshToken = _refreshTokensRepository.GetRefreshToken(user);
        if(refreshToken is null || token != refreshToken.Token || refreshToken.Expires < DateTime.Now)
        {
            throw new InvalidTokenException("Invalid refresh token");
        }
        bool succ = int.TryParse(_configuration.GetSection("Jwt:RefreshTokenExpireDays").Value, out int days);
        if (!succ)
        {
            throw new TokenCreationFailedException("Failed to retrieve token expiration days");
        }
        var newToken = Guid.NewGuid();
        var newExpiration = DateTime.Now.AddDays(days);
        if(!_refreshTokensRepository.ReplaceRefreshToken(refreshToken, newToken, newExpiration))
        {
            throw new TokenCreationFailedException("Failed to replace token");
        }
        return new RefreshTokenDto
        {
            Token = newToken.ToString(),
            Expires = newExpiration
        };
    }
}