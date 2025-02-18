using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.API.Services;

namespace TruckLoadingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TruckHistoryController : ControllerBase
    {
        private readonly TruckLocationService _truckLocationService;

        public TruckHistoryController(TruckLocationService truckLocationService)
        {
            _truckLocationService = truckLocationService;
        }

        [HttpGet("history/{truckId}")]
        public async Task<IActionResult> GetTruckHistory(int truckId)
        {
            var history = await _truckLocationService.GetTruckHistoryAsync(truckId);
            return history != null ? Ok(history) : NotFound(new { Message = "No history found for this truck." });
        }
    }
}
