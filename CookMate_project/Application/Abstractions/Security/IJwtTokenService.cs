using CookMate_project.Domain.Entities;

namespace CookMate_project.Application.Abstractions.Security;

public interface IJwtTokenService
{
    int ExpiresInSeconds { get; }
    string CreateToken(User user);
}
