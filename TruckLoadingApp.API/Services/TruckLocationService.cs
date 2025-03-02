using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Services
{
    public class TruckLocationService : ITruckLocationService
    {
        private readonly IDistributedCache _cache;
        private const string TRUCK_LOCATION_KEY = "TruckLocation_";
        private const string TRUCK_HISTORY_KEY = "TruckHistory_";

        public TruckLocationService(IDistributedCache cache)
        {
            _cache = cache;
        }

        // ✅ Save Current Location
        public async Task UpdateTruckLocationAsync(int truckId, decimal latitude, decimal longitude)
        {
            var truckLocation = new { TruckId = truckId, Latitude = latitude, Longitude = longitude, Timestamp = DateTime.UtcNow };
            string serializedData = JsonConvert.SerializeObject(truckLocation);

            // ✅ Store current location (overwrite previous)
            await _cache.SetStringAsync(TRUCK_LOCATION_KEY + truckId, serializedData);

            // ✅ Store history (append to list)
            var historyList = await GetTruckHistoryAsync(truckId) ?? new List<object>();
            historyList.Add(truckLocation);

            await _cache.SetStringAsync(TRUCK_HISTORY_KEY + truckId, JsonConvert.SerializeObject(historyList));
        }

        // ✅ Retrieve Current Location
        public async Task<string?> GetTruckLocationAsync(int truckId)
        {
            return await _cache.GetStringAsync(TRUCK_LOCATION_KEY + truckId);
        }

        // Implement the interface method
        public async Task<TruckLocation?> GetCurrentTruckLocationAsync(long truckId)
        {
            var locationJson = await _cache.GetStringAsync(TRUCK_LOCATION_KEY + truckId);
            if (string.IsNullOrEmpty(locationJson))
                return null;

            var locationData = JsonConvert.DeserializeObject<dynamic>(locationJson);
            
            return new TruckLocation
            {
                TruckId = (int)truckId,
                CurrentLatitude = locationData.Latitude,
                CurrentLongitude = locationData.Longitude,
                Timestamp = locationData.Timestamp
            };
        }

        // ✅ Retrieve Truck History
        public async Task<List<object>?> GetTruckHistoryAsync(int truckId)
        {
            var history = await _cache.GetStringAsync(TRUCK_HISTORY_KEY + truckId);
            return history != null ? JsonConvert.DeserializeObject<List<object>>(history) : null;
        }
    }
}
