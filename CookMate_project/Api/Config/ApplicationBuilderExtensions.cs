using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace CookMate_project.Api.Configuration;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UsePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true
        });

        return app;
    }
}
