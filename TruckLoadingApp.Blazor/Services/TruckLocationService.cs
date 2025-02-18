using Microsoft.AspNetCore.SignalR.Client;

namespace TruckLoadingApp.Blazor.Services
{
    public class TruckLocationService
    {
        private HubConnection? _hubConnection;
        public event Action<int, decimal, decimal>? OnTruckLocationUpdated;
        public event Action<int, string>? OnTruckArrived; // ✅ Add this event
        private Timer? _locationUpdateTimer;
        private Random _random = new Random(); // Simulate movement

        public async Task StartConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5281/truckhub") // ✅ Ensure it matches API
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<int, decimal, decimal>("ReceiveTruckLocation", (truckId, latitude, longitude) =>
            {
                OnTruckLocationUpdated?.Invoke(truckId, latitude, longitude);
            });

            _hubConnection.On<int, string>("TruckArrived", (truckId, message) =>
            {
                OnTruckArrived?.Invoke(truckId, message);
            });
            await _hubConnection.StartAsync();

            // ✅ Start automatic updates every 5 seconds
            _locationUpdateTimer = new Timer(async (state) =>
            {
                await SendTruckLocation(1, 37.7749m + (decimal)(_random.NextDouble() / 100), -122.4194m + (decimal)(_random.NextDouble() / 100));
            }, null, 0, 5000);
        }

        public async Task SendTruckLocation(int truckId, decimal latitude, decimal longitude)
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("UpdateTruckLocation", truckId, latitude, longitude);
            }
        }
    }
}
