using Contracts.Auth;
using CookMate_project.Application.Abstractions.Auth;
using CookMate_project.Application.Abstractions.Security;
using CookMate_project.Application.Common.Exceptions;
using CookMate_project.Application.Models;
using CookMate_project.Domain.Entities;
using CookMate_project.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CookMate_project.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshSessionRepository refreshSessionRepository,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService) : IAuthService
{
    // Register
    public async Task<AuthSessionResult> RegisterNewUserAsync(RegisterRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
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
        var session = CreateRefreshSession(user.Id, ipAddress, userAgent, null);
        var refreshToken = refreshTokenService.GenerateToken();
        session.TokenHash = refreshTokenService.HashToken(refreshToken);
        await refreshSessionRepository.AddAsync(session, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenService.CreateToken(user);
        return new AuthSessionResult(
            new AuthResponse(accessToken, jwtTokenService.ExpiresInSeconds),
            refreshToken,
            session.ExpiresAt);
    }

    // Login 
    public async Task<AuthSessionResult?> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
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
        var session = CreateRefreshSession(user.Id, ipAddress, userAgent, null);
        var refreshToken = refreshTokenService.GenerateToken();
        session.TokenHash = refreshTokenService.HashToken(refreshToken);
        await refreshSessionRepository.AddAsync(session, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenService.CreateToken(user);
        return new AuthSessionResult(
            new AuthResponse(accessToken, jwtTokenService.ExpiresInSeconds),
            refreshToken,
            session.ExpiresAt);
    }

    public async Task<AuthSessionResult?> RefreshAsync(string refreshToken, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var tokenHash = refreshTokenService.HashToken(refreshToken);
        var existingSession = await refreshSessionRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (existingSession is null)
        {
            return null;
        }

        if (existingSession.RevokedAt.HasValue)
        {
            if (!string.IsNullOrWhiteSpace(existingSession.ReplacedByTokenHash))    
            {
                await refreshSessionRepository.RevokeFamilyAsync(existingSession.FamilyId, now, ipAddress, cancellationToken);
            }

            return null;
        }

        if (existingSession.ExpiresAt <= now)
        {
            existingSession.RevokedAt = now;
            existingSession.RevokedByIp = ipAddress;
            await refreshSessionRepository.SaveChangesAsync(cancellationToken);
            return null;
        }

        var user = await userRepository.GetByIdAsync(existingSession.UserId, cancellationToken);
        if (user is null)
        {
            existingSession.RevokedAt = now;
            existingSession.RevokedByIp = ipAddress;
            await refreshSessionRepository.SaveChangesAsync(cancellationToken);
            return null;
        }

        var nextRefreshToken = refreshTokenService.GenerateToken();
        var nextTokenHash = refreshTokenService.HashToken(nextRefreshToken);

        existingSession.RevokedAt = now;
        existingSession.RevokedByIp = ipAddress;
        existingSession.ReplacedByTokenHash = nextTokenHash;

        var newSession = CreateRefreshSession(existingSession.UserId, ipAddress, userAgent, existingSession.FamilyId);
        newSession.TokenHash = nextTokenHash;

        await refreshSessionRepository.AddAsync(newSession, cancellationToken);
        await refreshSessionRepository.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenService.CreateToken(user);
        return new AuthSessionResult(
            new AuthResponse(accessToken, jwtTokenService.ExpiresInSeconds),
            nextRefreshToken,
            newSession.ExpiresAt);
    }

    public async Task LogoutAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var tokenHash = refreshTokenService.HashToken(refreshToken);
        var session = await refreshSessionRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (session is null || session.RevokedAt.HasValue)
        {
            return;
        }

        session.RevokedAt = DateTime.UtcNow;
        session.RevokedByIp = ipAddress;
        await refreshSessionRepository.SaveChangesAsync(cancellationToken);
    }

    public Task LogoutAllAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default)
        => refreshSessionRepository.RevokeAllForUserAsync(userId, DateTime.UtcNow, ipAddress, cancellationToken);

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

    private RefreshSession CreateRefreshSession(Guid userId, string? ipAddress, string? userAgent, Guid? familyId)
    {
        var now = DateTime.UtcNow;
        return new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FamilyId = familyId ?? Guid.NewGuid(),
            ExpiresAt = refreshTokenService.GetExpiresAtUtc(now),
            CreatedAt = now,
            CreatedByIp = ipAddress,
            UserAgent = userAgent
        };
    }
}
