using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
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

    // Implement methods used by TruckController
    public async Task<IEnumerable<Truck>> GetAllTrucksAsync()
    {
        return await _context.Trucks
            .Include(t => t.TruckType)
            .Include(t => t.AssignedDriver)
            .ToListAsync();
    }

    public async Task<Truck?> GetTruckByIdAsync(long id)
    {
        return await _context.Trucks
            .Include(t => t.TruckType)
            .Include(t => t.AssignedDriver)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Truck> CreateTruckAsync(Truck truck)
    {
        truck.CreatedDate = DateTime.UtcNow;
        truck.OperationalStatus = TruckOperationalStatusEnum.AwaitingApproval;
        
        _context.Trucks.Add(truck);
        await _context.SaveChangesAsync();
        
        return truck;
    }

    public async Task<bool> UpdateTruckAsync(Truck truck)
    {
        var existingTruck = await _context.Trucks.FindAsync(truck.Id);
        if (existingTruck == null) return false;
        
        existingTruck.TruckTypeId = truck.TruckTypeId;
        existingTruck.NumberPlate = truck.NumberPlate;
        existingTruck.LoadCapacityWeight = truck.LoadCapacityWeight;
        existingTruck.LoadCapacityVolume = truck.LoadCapacityVolume;
        existingTruck.Height = truck.Height;
        existingTruck.Width = truck.Width;
        existingTruck.Length = truck.Length;
        existingTruck.AvailabilityStartDate = truck.AvailabilityStartDate;
        existingTruck.AvailabilityEndDate = truck.AvailabilityEndDate;
        existingTruck.OperationalStatus = truck.OperationalStatus;
        existingTruck.AssignedDriverId = truck.AssignedDriverId;
        existingTruck.AvailableCapacityWeight = truck.AvailableCapacityWeight;
        existingTruck.UpdatedDate = DateTime.UtcNow;
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTruckAsync(long id)
    {
        var truck = await _context.Trucks.FindAsync(id);
        if (truck == null) return false;
        
        _context.Trucks.Remove(truck);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Truck>> GetAvailableTrucksAsync()
    {
        return await _context.Trucks
            .Where(t => t.IsApproved && 
                   t.OperationalStatus == TruckOperationalStatusEnum.Active &&
                   t.AvailabilityStartDate <= DateTime.UtcNow &&
                   t.AvailabilityEndDate >= DateTime.UtcNow)
            .Include(t => t.TruckType)
            .Include(t => t.AssignedDriver)
            .ToListAsync();
    }
}
