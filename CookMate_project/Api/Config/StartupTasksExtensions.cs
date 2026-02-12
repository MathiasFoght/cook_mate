using CookMate_project.Domain.Entities;
using CookMate_project.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CookMate_project.Api.Configuration;

public static class StartupTasksExtensions
{
    public static async Task RunStartupAsync(this WebApplication app)
    {
        if (app.Configuration.GetValue<bool>("AUTO_MIGRATE"))
        {
            await ApplyMigrationsAsync(app);
        }

        await SeedDevelopmentTestUserAsync(app);
    }

    private static async Task ApplyMigrationsAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    private static async Task SeedDevelopmentTestUserAsync(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        var enabled = app.Configuration.GetValue<bool>("SeedTestUser:Enabled");
        if (!enabled)
        {
            return;
        }

        var email = app.Configuration["SeedTestUser:Email"]?.Trim().ToLowerInvariant();
        var password = app.Configuration["SeedTestUser:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            app.Logger.LogWarning("SeedTestUser is enabled but Email/Password is missing.");
            return;
        }

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var exists = await dbContext.Users.AnyAsync(x => x.Email == email);
        if (exists)
        {
            return;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        app.Logger.LogInformation("Seeded development test user: {Email}", email);
    }
}
