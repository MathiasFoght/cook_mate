namespace CookMate_project.Application.Abstractions.Security;

public interface IRefreshTokenService
{
    DateTime GetExpiresAtUtc(DateTime fromUtc);
    string GenerateToken();
    string HashToken(string token);
}
