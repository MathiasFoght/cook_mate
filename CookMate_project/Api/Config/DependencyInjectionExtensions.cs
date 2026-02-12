using CookMate_project.Application.Abstractions.Auth;
using CookMate_project.Application.Abstractions.Security;
using CookMate_project.Application.Services;
using CookMate_project.Domain.Entities;
using CookMate_project.Domain.Repositories;
using CookMate_project.Infrastructure.Persistence;
using CookMate_project.Infrastructure.Persistence.Repositories;
using CookMate_project.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CookMate_project.Api.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection ServiceCollection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddSwagger();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings__DefaultConnection");

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                         ?? throw new InvalidOperationException("Missing JWT config section: Jwt");
        jwtOptions.Validate();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.RegisterSingleton(jwtOptions);

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");

        services.RegisterScoped<IUserRepository, UserRepository>();
        services.RegisterScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.RegisterScoped<IJwtTokenService, JwtTokenService>();
        services.RegisterScoped<IAuthService, AuthService>();

        services.AddAuthentication(jwtOptions);
        services.AddAuthorization();

        return services;
    }
}
