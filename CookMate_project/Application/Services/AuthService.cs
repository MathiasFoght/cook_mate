using Contracts.Auth;
using CookMate_project.Application.Abstractions.Auth;
using CookMate_project.Application.Abstractions.Security;
using CookMate_project.Application.Common.Exceptions;
using CookMate_project.Domain.Entities;
using CookMate_project.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CookMate_project.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthService
{
    // Register
    public async Task<AuthResponse> RegisterNewUserAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCredentials(request.Email, request.Password);

        var email = NormalizeEmail(request.Email);

        var exists = await userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (exists)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        // Password hash
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.CreateToken(user);
        return new AuthResponse(token, jwtTokenService.ExpiresInSeconds);
    }

    // Login 
    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCredentials(request.Email, request.Password);

        var email = NormalizeEmail(request.Email);
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.CreateToken(user);
        return new AuthResponse(token, jwtTokenService.ExpiresInSeconds);
    }

    // Get Profile
    public async Task<UserProfileResponse?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        return user is null
            ? null
            : new UserProfileResponse(user.Id, user.Email, user.CreatedAt, user.LastLoginAt);
    }

    // Helper: Validate user credentials
    private static void ValidateCredentials(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("Email and password are required.");
        }

        var normalizedEmail = NormalizeEmail(email);

        if (!normalizedEmail.Contains('@'))
        {
            throw new ValidationException("Email is invalid.");
        }

        if (password.Length < 8)
        {
            throw new ValidationException("Password must be at least 8 characters.");
        }
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
