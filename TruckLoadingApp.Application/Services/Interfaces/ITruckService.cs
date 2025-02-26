using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ITruckService
    {
        Task<bool> RegisterTruck(Truck truck, string ownerId, bool isIndividualTrucker);
        Task<IEnumerable<Truck>> GetUnapprovedTrucks();
        Task<bool> ApproveTruck(int truckId);
        Task<Truck?> GetTruckById(int truckId);
        // 🚀 Add missing method
        Task<Truck?> GetTruckByOwnerId(string ownerId);
    }
}
