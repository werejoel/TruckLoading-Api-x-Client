namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ITruckService
    {
        Task<bool> RegisterTruck(Truck truck, string ownerId, bool isIndividualTrucker);
        Task<IEnumerable<Truck>> GetUnapprovedTrucks();
        Task<bool> ApproveTruck(int truckId);
        Task<Truck?> GetTruckById(int truckId);
        Task<Truck?> GetTruckByOwnerId(string ownerId);
        
        // Add methods used by TruckController
        Task<IEnumerable<Truck>> GetAllTrucksAsync();
        Task<Truck?> GetTruckByIdAsync(long id);
        Task<Truck> CreateTruckAsync(Truck truck);
        Task<bool> UpdateTruckAsync(Truck truck);
        Task<bool> DeleteTruckAsync(long id);
        Task<IEnumerable<Truck>> GetAvailableTrucksAsync();
    }
}
