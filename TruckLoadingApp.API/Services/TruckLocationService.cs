using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace TruckLoadingApp.API.Services
{
    public class TruckLocationService
    {
        private readonly IDistributedCache _cache;
        private const string TRUCK_LOCATION_KEY = "TruckLocation_";

        public TruckLocationService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task UpdateTruckLocationAsync(int truckId, decimal latitude, decimal longitude)
        {
            var truckLocation = new { TruckId = truckId, Latitude = latitude, Longitude = longitude };
            string serializedData = JsonConvert.SerializeObject(truckLocation);

            await _cache.SetStringAsync(TRUCK_LOCATION_KEY + truckId, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Store for 10 minutes
            });
        }

        public async Task<string?> GetTruckLocationAsync(int truckId)
        {
            return await _cache.GetStringAsync(TRUCK_LOCATION_KEY + truckId);
        }
    }
}
