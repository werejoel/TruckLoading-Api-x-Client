using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Infrastructure.Data;

public class TruckService : ITruckService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TruckService> _logger;
    private readonly IDriverService _driverService;

    public TruckService(ApplicationDbContext context, ILogger<TruckService> logger, IDriverService driverService)
    {
        _context = context;
        _logger = logger;
        _driverService = driverService;
    }

    public async Task<bool> RegisterTruck(Truck truck, string ownerId, bool isIndividualTrucker)
    {
        // 🚀 Ensure truckers can only register ONE truck
        if (isIndividualTrucker)
        {
            var existingTruck = await _context.Trucks.FirstOrDefaultAsync(t => t.OwnerId == ownerId);
            if (existingTruck != null)
            {
                _logger.LogWarning($"Trucker {ownerId} attempted to register multiple trucks.");
                return false; // ❌ Prevents multiple truck registrations
            }
        }

        truck.OwnerId = ownerId;
        truck.IsApproved = false;
        truck.OperationalStatus = TruckOperationalStatusEnum.AwaitingApproval;

        _context.Trucks.Add(truck);
        var result = await _context.SaveChangesAsync() > 0;

        if (result && isIndividualTrucker)
        {
            _logger.LogInformation($"Assigning owner {ownerId} as driver for their truck.");
            await _driverService.AssignOwnerAsDriver(ownerId, truck.Id);
        }

        return result;
    }


    public async Task<IEnumerable<Truck>> GetUnapprovedTrucks()
    {
        return await _context.Trucks
            .Where(t => !t.IsApproved)
            .ToListAsync();
    }

    public async Task<bool> ApproveTruck(int truckId)
    {
        var truck = await _context.Trucks.FindAsync(truckId);
        if (truck == null) return false;

        truck.IsApproved = true;
        truck.OperationalStatus = TruckOperationalStatusEnum.Active;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Truck?> GetTruckById(int truckId)
    {
        return await _context.Trucks.FindAsync(truckId);
    }

    public async Task<Truck?> GetTruckByOwnerId(string ownerId)
    {
        return await _context.Trucks.FirstOrDefaultAsync(t => t.OwnerId == ownerId);
    }

}
