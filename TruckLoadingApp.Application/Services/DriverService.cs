using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            driver.UserId = userId;
            driver.IsAvailable = true;

            _context.Drivers.Add(driver);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignDriverToTruck(long driverId, int truckId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
            if (driver == null) return false;

            var truck = await _context.Trucks.FirstOrDefaultAsync(t => t.Id == truckId);
            if (truck == null || !truck.IsApproved) return false;

            // 🚀 Ensure only company-registered trucks allow driver assignment
            var truckOwner = await _context.Users.FindAsync(truck.OwnerId);
            if (truckOwner == null || truckOwner.UserType != UserType.Company)
            {
                _logger.LogWarning($"Driver assignment denied. Truck {truckId} is not owned by a company.");
                return false; // ❌ Prevents assigning drivers to trucks owned by individual truckers
            }

            // Ensure truck does not already have a driver
            if (await _context.Drivers.AnyAsync(d => d.TruckId == truckId))
                return false;

            driver.TruckId = truckId;
            driver.IsAvailable = false;
            truck.AssignedDriver = driver;

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> UnassignDriver(long driverId)
        {
            var driver = await _context.Drivers.FindAsync(driverId);
            if (driver == null) return false;

            driver.TruckId = null;
            driver.IsAvailable = true;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Driver>> GetAvailableDrivers()
        {
            return await _context.Drivers
                .Where(d => d.IsAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<Driver>> GetDriversByCompany(string companyId)
        {
            return await _context.Drivers
                .Where(d => _context.Trucks.Any(t => t.OwnerId == companyId && t.Id == d.TruckId))
                .ToListAsync();
        }

        public async Task<bool> AssignOwnerAsDriver(string ownerId, int truckId)
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
    }
}
