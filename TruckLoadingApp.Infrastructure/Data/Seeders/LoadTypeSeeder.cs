using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data.Seeders
{
    public class LoadTypeSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoadTypeSeeder> _logger;

        public LoadTypeSeeder(ApplicationDbContext context, ILogger<LoadTypeSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Check if data already exists
                if (await _context.LoadTypes.AnyAsync())
                {
                    _logger.LogInformation("LoadTypes data already exists, skipping seeding.");
                    return;
                }

                _logger.LogInformation("Starting to seed LoadTypes...");

                // Add load types
                var loadTypes = new List<LoadType>
                {
                    new LoadType 
                    { 
                        Name = "General Cargo",
                        Description = "Standard non-specialized cargo that doesn't require special handling",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        HasSpecialHandling = false,
                        IsHazardous = false,
                        RequiresRefrigeration = false
                    },
                    new LoadType 
                    { 
                        Name = "Refrigerated Goods",
                        Description = "Temperature-sensitive items requiring refrigeration",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        HasSpecialHandling = true,
                        IsHazardous = false,
                        RequiresRefrigeration = true
                    },
                    new LoadType 
                    { 
                        Name = "Hazardous Materials",
                        Description = "Dangerous goods requiring special handling and documentation",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        HasSpecialHandling = true,
                        IsHazardous = true,
                        RequiresRefrigeration = false
                    },
                    new LoadType 
                    { 
                        Name = "Fragile Items",
                        Description = "Delicate items requiring careful handling",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        HasSpecialHandling = true,
                        IsHazardous = false,
                        RequiresRefrigeration = false
                    },
                    new LoadType 
                    { 
                        Name = "Heavy Machinery",
                        Description = "Large industrial equipment and machinery",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        HasSpecialHandling = true,
                        IsHazardous = false,
                        RequiresRefrigeration = false
                    },
                    new LoadType 
                    { 
                        Name = "Bulk Materials",
                        Description = "Loose materials transported in large quantities",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        HasSpecialHandling = false,
                        IsHazardous = false,
                        RequiresRefrigeration = false
                    }
                };

                await _context.LoadTypes.AddRangeAsync(loadTypes);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded {Count} LoadTypes.", loadTypes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding LoadTypes.");
                throw;
            }
        }
    }
} 