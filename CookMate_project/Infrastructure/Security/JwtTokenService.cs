using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CookMate_project.Application.Abstractions.Security;
using CookMate_project.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CookMate_project.Infrastructure.Security;

public class JwtTokenService(JwtOptions jwtOptions) : IJwtTokenService
{
    public int ExpiresInSeconds => jwtOptions.AccessTokenMinutes * 60;

    public string CreateToken(User user)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            Expires = now.AddMinutes(jwtOptions.AccessTokenMinutes),
            NotBefore = now,
            SigningCredentials = new SigningCredentials(jwtOptions.GetSigningKey(), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);
    }
}
