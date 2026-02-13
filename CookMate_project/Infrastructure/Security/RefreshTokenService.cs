using System.Security.Cryptography;
using System.Text;
using CookMate_project.Application.Abstractions.Security;
using Microsoft.AspNetCore.WebUtilities;

namespace CookMate_project.Infrastructure.Security;

public class RefreshTokenService(RefreshTokenOptions options) : IRefreshTokenService
{
    public DateTime GetExpiresAtUtc(DateTime fromUtc) => fromUtc.AddDays(options.RefreshTokenDays);

    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    public string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
