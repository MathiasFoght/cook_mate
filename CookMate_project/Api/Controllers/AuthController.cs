using System.Security.Claims;
using Contracts.Auth;
using CookMate_project.Application.Abstractions.Auth;
using CookMate_project.Application.Common.Exceptions;
using CookMate_project.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookMate_project.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, RefreshTokenOptions refreshTokenOptions) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RegisterNewUserAsync(
                request,
                GetIpAddress(),
                GetUserAgent(),
                cancellationToken);
            AppendRefreshCookie(response.RefreshToken, response.RefreshTokenExpiresAt);
            return Ok(response.Response);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.LoginAsync(
                request,
                GetIpAddress(),
                GetUserAgent(),
                cancellationToken);
            if (response is null)
            {
                return Unauthorized();
            }

            AppendRefreshCookie(response.RefreshToken, response.RefreshTokenExpiresAt);
            return Ok(response.Response);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken cancellationToken)
    {
        if (!TryGetRefreshToken(out var refreshToken))
        {
            return Unauthorized();
        }

        var response = await authService.RefreshAsync(
            refreshToken,
            GetIpAddress(),
            GetUserAgent(),
            cancellationToken);

        if (response is null)
        {
            ClearRefreshCookie();
            return Unauthorized();
        }

        AppendRefreshCookie(response.RefreshToken, response.RefreshTokenExpiresAt);
        return Ok(response.Response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (TryGetRefreshToken(out var refreshToken))
        {
            await authService.LogoutAsync(refreshToken, GetIpAddress(), cancellationToken);
        }

        ClearRefreshCookie();
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        await authService.LogoutAllAsync(userId, GetIpAddress(), cancellationToken);
        ClearRefreshCookie();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> Me(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var response = await authService.GetProfileAsync(userId, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    private bool TryGetRefreshToken(out string refreshToken)
    {
        var hasCookie = Request.Cookies.TryGetValue(refreshTokenOptions.CookieName, out var cookieValue);
        refreshToken = cookieValue ?? string.Empty;
        return hasCookie && !string.IsNullOrWhiteSpace(refreshToken);
    }

    private void AppendRefreshCookie(string refreshToken, DateTime expiresAtUtc)
    {
        Response.Cookies.Append(refreshTokenOptions.CookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = refreshTokenOptions.CookieSecure,
            SameSite = SameSiteMode.Strict,
            Path = refreshTokenOptions.CookiePath,
            Expires = new DateTimeOffset(expiresAtUtc),
            IsEssential = true
        });
    }

    private void ClearRefreshCookie()
    {
        Response.Cookies.Delete(refreshTokenOptions.CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = refreshTokenOptions.CookieSecure,
            SameSite = SameSiteMode.Strict,
            Path = refreshTokenOptions.CookiePath,
            IsEssential = true
        });
    }

    private string? GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString();

    private string? GetUserAgent() => Request.Headers.UserAgent.ToString();
}
