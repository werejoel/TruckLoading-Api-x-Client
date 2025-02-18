using Microsoft.AspNetCore.SignalR.Client;

namespace TruckLoadingApp.Blazor.Services
{
    public class TruckLocationService
    {
        private HubConnection? _hubConnection;
        public event Action<int, decimal, decimal>? OnTruckLocationUpdated;

        public async Task StartConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5281/truckHub") // Update with your API URL
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<int, decimal, decimal>("ReceiveTruckLocation", (truckId, latitude, longitude) =>
            {
                OnTruckLocationUpdated?.Invoke(truckId, latitude, longitude);
            });

            await _hubConnection.StartAsync();
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
