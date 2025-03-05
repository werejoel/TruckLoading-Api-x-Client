using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Infrastructure.Data;
using TruckLoadingApp.Infrastructure.Data.Seeders;

namespace TruckLoadingApp.Infrastructure.Extensions
{
    // Non-static class for logging purposes
    internal class DataSeederLogger { }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataSeeders(this IServiceCollection services)
        {
            services.AddScoped<TruckDataSeeder>();
            services.AddScoped<LoadTypeSeeder>();
            return services;
        }

        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                
                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                var truckDataSeeder = services.GetRequiredService<TruckDataSeeder>();
                await truckDataSeeder.SeedAsync();

                var loadTypeSeeder = services.GetRequiredService<LoadTypeSeeder>();
                await loadTypeSeeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<DataSeederLogger>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
} 