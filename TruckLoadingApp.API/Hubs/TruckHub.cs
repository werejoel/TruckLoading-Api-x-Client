using Microsoft.AspNetCore.SignalR;
using TruckLoadingApp.API.Services;

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
            // Save location in Redis (or memory)
            await _truckLocationService.UpdateTruckLocationAsync(truckId, latitude, longitude);

            // Notify all clients about the new truck location
            await Clients.All.SendAsync("ReceiveTruckLocation", truckId, latitude, longitude);
        }
    }
}
