namespace CookMate_project.Infrastructure.Security;

public class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    public int RefreshTokenDays { get; set; } = 30;
    public string CookieName { get; set; } = "cookmate_rt";
    public string CookiePath { get; set; } = "/api/auth";
    public bool CookieSecure { get; set; } = true;

    public void Validate()
    {
        if (RefreshTokenDays <= 0)
        {
            throw new InvalidOperationException("RefreshToken:RefreshTokenDays must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(CookieName))
        {
            throw new InvalidOperationException("RefreshToken:CookieName is required.");
        }

        if (string.IsNullOrWhiteSpace(CookiePath))
        {
            throw new InvalidOperationException("RefreshToken:CookiePath is required.");
        }
    }
}
