namespace Contracts.Auth;

public record AuthResponse(string AccessToken, int ExpiresInSeconds);
