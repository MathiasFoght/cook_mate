using Contracts.Auth;

namespace CookMate_project.Application.Abstractions.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterNewUserAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserProfileResponse?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}
