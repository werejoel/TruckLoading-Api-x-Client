using Microsoft.AspNetCore.SignalR;
using TruckLoadingApp.API.Services;
using System.Threading.Tasks;

namespace TruckLoadingApp.API.Hubs
{
    public class TruckHub : Hub
    {
        private readonly TruckLocationService _truckLocationService;

        public TruckHub(TruckLocationService truckLocationService)
        {
            _truckLocationService = truckLocationService;
        }

        public async Task UpdateTruckLocation(int truckId, decimal latitude, decimal longitude)
        {
            // ✅ Save truck location
            await _truckLocationService.UpdateTruckLocationAsync(truckId, latitude, longitude);

            // ✅ Notify all clients of the updated location
            await Clients.All.SendAsync("ReceiveTruckLocation", truckId, latitude, longitude);

            // ✅ Check if the truck has reached its destination
            var destination = new { Latitude = 37.7750m, Longitude = -122.4190m }; // Example destination
            if (Math.Abs(destination.Latitude - latitude) < 0.01m && Math.Abs(destination.Longitude - longitude) < 0.01m)
            {
                await Clients.All.SendAsync("TruckArrived", truckId, $"Truck {truckId} has reached its destination!");
            }
        }
    }
}
