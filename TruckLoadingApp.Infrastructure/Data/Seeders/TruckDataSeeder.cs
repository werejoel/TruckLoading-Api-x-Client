using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data.Seeders
{
    public class TruckDataSeeder
    {
        private readonly ApplicationDbContext _context;

        public TruckDataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Check if data already exists
            if (await _context.TruckCategories.AnyAsync() || await _context.TruckTypes.AnyAsync())
            {
                return; // Data already exists, skip seeding
            }

            // Add categories
            var categories = new List<TruckCategory>
            {
                new TruckCategory { CategoryName = "Small Trucks (Light-Duty Cargo Transport)", CreatedDate = DateTime.UtcNow },
                new TruckCategory { CategoryName = "Medium-Sized Trucks (Medium-Duty Cargo Transport)", CreatedDate = DateTime.UtcNow },
                new TruckCategory { CategoryName = "Large Trucks (Heavy-Duty Cargo Transport)", CreatedDate = DateTime.UtcNow }
            };

            await _context.TruckCategories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Get the categories with their assigned IDs
            var smallTruckCategory = await _context.TruckCategories.FirstAsync(c => c.CategoryName.Contains("Small Trucks"));
            var mediumTruckCategory = await _context.TruckCategories.FirstAsync(c => c.CategoryName.Contains("Medium-Sized Trucks"));
            var largeTruckCategory = await _context.TruckCategories.FirstAsync(c => c.CategoryName.Contains("Large Trucks"));

            // Add truck types
            var truckTypes = new List<TruckType>
            {
                // Small Trucks (Light-Duty)
                new TruckType 
                { 
                    Name = "Pickup Truck", 
                    CategoryId = smallTruckCategory.Id, 
                    Description = "Used for small cargo loads and short distances.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Mini Truck (Kei Truck)", 
                    CategoryId = smallTruckCategory.Id, 
                    Description = "Compact truck for light loads, common in urban areas.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Panel Van", 
                    CategoryId = smallTruckCategory.Id, 
                    Description = "Fully enclosed for secure small cargo transport.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Refrigerated Van (Chiller Van)", 
                    CategoryId = smallTruckCategory.Id, 
                    Description = "Used for transporting perishable goods.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Flatbed Mini Truck", 
                    CategoryId = smallTruckCategory.Id, 
                    Description = "Open cargo area, ideal for small construction materials.",
                    CreatedDate = DateTime.UtcNow 
                },

                // Medium-Sized Trucks (Medium-Duty)
                new TruckType 
                { 
                    Name = "Box Truck (Straight Truck)", 
                    CategoryId = mediumTruckCategory.Id, 
                    Description = "Enclosed cargo area for furniture, appliances, and parcels.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Refrigerated Truck (Reefer Truck)", 
                    CategoryId = mediumTruckCategory.Id, 
                    Description = "Temperature-controlled truck for food and medicine.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Stake Truck (Sideboard Truck)", 
                    CategoryId = mediumTruckCategory.Id, 
                    Description = "Open cargo bed with removable side panels for easy loading.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Dump Truck (Tipper Truck)", 
                    CategoryId = mediumTruckCategory.Id, 
                    Description = "Carries loose materials like sand, gravel, and dirt.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Curtainsider Truck", 
                    CategoryId = mediumTruckCategory.Id, 
                    Description = "Has sliding curtains on the sides for easy loading/unloading.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Tanker Truck (Medium Size)", 
                    CategoryId = mediumTruckCategory.Id, 
                    Description = "Used for transporting liquids such as milk or fuel in moderate quantities.",
                    CreatedDate = DateTime.UtcNow 
                },

                // Large Trucks (Heavy-Duty)
                new TruckType 
                { 
                    Name = "Semi-Truck (Tractor-Trailer / 18-Wheeler)", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Standard for long-haul freight transport.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Flatbed Truck", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Open bed used for heavy and oversized cargo.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Lowboy Trailer", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Designed for transporting heavy equipment like bulldozers.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Tanker Truck (Large Capacity)", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Hauls fuel, chemicals, and other liquids in bulk.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Logging Truck", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Specially designed for transporting logs.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Car Carrier (Auto Transporter)", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Used for moving multiple vehicles.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Container Truck", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Hauls shipping containers from ports to warehouses.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Side Loader Truck", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Used for transporting and loading shipping containers.",
                    CreatedDate = DateTime.UtcNow 
                },
                new TruckType 
                { 
                    Name = "Livestock Truck", 
                    CategoryId = largeTruckCategory.Id, 
                    Description = "Used for transporting animals with ventilation and safety enclosures.",
                    CreatedDate = DateTime.UtcNow 
                }
            };

            await _context.TruckTypes.AddRangeAsync(truckTypes);
            await _context.SaveChangesAsync();
        }
    }
} 