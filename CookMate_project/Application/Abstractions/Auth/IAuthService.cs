using Contracts.Auth;
using CookMate_project.Application.Models;

namespace CookMate_project.Application.Abstractions.Auth;

public interface IAuthService
{
    Task<AuthSessionResult> RegisterNewUserAsync(RegisterRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
    Task<AuthSessionResult?> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
    Task<AuthSessionResult?> RefreshAsync(string refreshToken, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
    Task LogoutAllAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default);
    Task<UserProfileResponse?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}
