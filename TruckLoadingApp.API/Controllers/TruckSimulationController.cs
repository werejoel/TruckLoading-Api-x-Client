using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TruckLoadingApp.API.Hubs;

namespace TruckLoadingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TruckSimulationController : ControllerBase
    {
        private readonly IHubContext<TruckHub> _hubContext;

        public TruckSimulationController(IHubContext<TruckHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("updateLocation")]
        public async Task<IActionResult> SimulateTruckLocation(int truckId, decimal latitude, decimal longitude)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveTruckLocation", truckId, latitude, longitude);
            return Ok(new { Message = "Truck location updated successfully!" });
        }
    }
}
