using Contracts.Auth;

namespace CookMate_project.Application.Models;

public record AuthSessionResult(AuthResponse Response, string RefreshToken, DateTime RefreshTokenExpiresAt);
