using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DriverService> _logger;

        public DriverService(ApplicationDbContext context, ILogger<DriverService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> RegisterDriver(Driver driver, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (await _context.Drivers.AnyAsync(d => d.UserId == userId))
                throw new InvalidOperationException("Driver already exists for this user");

            driver.UserId = userId;
            driver.IsAvailable = true;
            driver.CreatedDate = DateTime.UtcNow;

            // Initialize collections
            driver.Documents = new List<DriverDocument>();
            driver.Performances = new List<DriverPerformance>();
            driver.Schedules = new List<DriverSchedule>();
            driver.RestPeriods = new List<DriverRestPeriod>();

            _context.Drivers.Add(driver);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignDriverToTruckAsync(long driverId, long truckId)
        {
            var driver = await _context.Drivers
                .Include(d => d.Truck)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new KeyNotFoundException($"Driver with ID {driverId} not found");

            var truck = await _context.Trucks
                .Include(t => t.AssignedDriver)
                .FirstOrDefaultAsync(t => t.Id == truckId);

            if (truck == null || !truck.IsApproved)
                throw new InvalidOperationException($"Truck with ID {truckId} not found or not approved");

            // Ensure only company-registered trucks allow driver assignment
            var truckOwner = await _context.Users.FindAsync(truck.OwnerId);
            if (truckOwner == null || truckOwner.UserType != UserType.Company)
            {
                throw new InvalidOperationException($"Truck {truckId} is not owned by a company");
            }

            // Ensure truck does not already have a driver
            if (truck.AssignedDriver != null)
                throw new InvalidOperationException($"Truck {truckId} already has an assigned driver");

            driver.TruckId = (int)truckId; // Cast to int since TruckId is int in the model
            driver.IsAvailable = false;
            driver.UpdatedDate = DateTime.UtcNow;

            truck.AssignedDriver = driver;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UnassignDriverAsync(long driverId)
        {
            var driver = await _context.Drivers
                .Include(d => d.Truck)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new KeyNotFoundException($"Driver with ID {driverId} not found");

            if (driver.Truck != null)
            {
                driver.Truck.AssignedDriver = null;
            }

            driver.TruckId = null;
            driver.IsAvailable = true;
            driver.UpdatedDate = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Driver>> GetAvailableDriversAsync()
        {
            return await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Truck)
                .Include(d => d.RoutePreferences)
                .Include(d => d.Performances.OrderByDescending(p => p.Date).Take(1))
                .Where(d => d.IsAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<Driver>> GetDriversByCompanyAsync(string companyId)
        {
            if (string.IsNullOrEmpty(companyId))
                throw new ArgumentException("Company ID cannot be empty", nameof(companyId));

            return await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Truck)
                .Include(d => d.RoutePreferences)
                .Include(d => d.Performances.OrderByDescending(p => p.Date).Take(1))
                .Where(d => _context.Trucks.Any(t => t.OwnerId == companyId && t.Id == d.TruckId))
                .ToListAsync();
        }

        public async Task<bool> AssignOwnerAsDriverAsync(string ownerId, int truckId)
        {
            var driver = new Driver
            {
                UserId = ownerId,
                LicenseNumber = "OWNER-DEFAULT", // 🚀 Needs real license later
                LicenseExpiryDate = DateTime.UtcNow.AddYears(5),
                IsAvailable = false,
                TruckId = truckId
            };

            _context.Drivers.Add(driver);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Truck)
                .ToListAsync();
        }

        public async Task<Driver?> GetDriverByIdAsync(long id)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Truck)
                .Include(d => d.Documents)
                .Include(d => d.Certifications)
                .Include(d => d.Performances)
                .Include(d => d.RoutePreferences)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Driver> CreateDriverAsync(Driver driver)
        {
            driver.CreatedDate = DateTime.UtcNow;
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return driver;
        }

        public async Task<bool> UpdateDriverAsync(Driver driver)
        {
            var existingDriver = await _context.Drivers.FindAsync(driver.Id);
            if (existingDriver == null)
                return false;

            driver.UpdatedDate = DateTime.UtcNow;
            _context.Entry(existingDriver).CurrentValues.SetValues(driver);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDriverAsync(long id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return false;

            _context.Drivers.Remove(driver);
            return await _context.SaveChangesAsync() > 0;
        }

        // Synchronous methods that delegate to their async counterparts
        public Task<bool> AssignDriverToTruck(long driverId, int truckId)
        {
            return AssignDriverToTruckAsync(driverId, (long)truckId);
        }

        public Task<bool> UnassignDriver(long driverId)
        {
            return UnassignDriverAsync(driverId);
        }

        public Task<IEnumerable<Driver>> GetAvailableDrivers()
        {
            return GetAvailableDriversAsync();
        }

        public Task<IEnumerable<Driver>> GetDriversByCompany(string companyId)
        {
            return GetDriversByCompanyAsync(companyId);
        }

        public Task<bool> AssignOwnerAsDriver(string ownerId, int truckId)
        {
            return AssignOwnerAsDriverAsync(ownerId, truckId);
        }
    }
}
