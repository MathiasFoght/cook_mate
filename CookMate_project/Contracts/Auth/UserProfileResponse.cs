namespace Contracts.Auth;

public record UserProfileResponse(Guid Id, string Email, DateTime CreatedAt, DateTime? LastLoginAt);
