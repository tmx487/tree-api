using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Claims;
using System.Text;
using api.Domain.Abstractions;
using api.Presentation.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.Infrastructure.Services;

public class JwtGenerator : IJwtGenerator
{
    private const int TokenLifetimeMinutes = 5;
    
    private readonly IConfiguration _configuration;

    public JwtGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenInfo GenerateToken(IEnumerable<Claim> claims)
    {
        var secret = _configuration["JwtSettings:Key"]; 
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expiresInMinutes = _configuration["JwtSettings:ExpiresInMinutes"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationTime = double.TryParse(expiresInMinutes, out var minutes) ? minutes : TokenLifetimeMinutes;
        var expires = DateTime.UtcNow.AddMinutes(expirationTime); 

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = credentials,
            Issuer = issuer,
            Audience = audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        
        var tokenString = tokenHandler.WriteToken(securityToken);

        return new TokenInfo(tokenString);
    }
}