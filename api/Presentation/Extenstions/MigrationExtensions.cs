using Microsoft.EntityFrameworkCore;

namespace api.Presentation.Extenstions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync<TContext>(this IHost host) 
        where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        var logger = services.GetRequiredService<ILogger<TContext>>();
        
        try
        {
            var dbContext = services.GetRequiredService<TContext>();
            
            logger.LogInformation("Applying database migrations programmatically...");
            
            await dbContext.Database.MigrateAsync();
            
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database for context {DbContextName}.", 
                typeof(TContext).Name);
        }
    }
}