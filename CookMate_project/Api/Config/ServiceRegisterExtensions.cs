using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CookMate_project.Api.Configuration;

public static class ServiceRegisterExtensions
{
    public static IServiceCollection RegisterScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddScoped<TService, TImplementation>();
        return services;
    }

    public static IServiceCollection RegisterSingleton<TService>(this IServiceCollection services, TService implementation)
        where TService : class
    {
        services.TryAddSingleton(implementation);
        return services;
    }
}
