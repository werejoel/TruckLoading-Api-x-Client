using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.API.Services;

namespace TruckLoadingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TruckHistoryController : ControllerBase
    {
        private readonly TruckLocationService _truckLocationService;
        private readonly ILogger<TruckHistoryController> _logger;

        public TruckHistoryController(TruckLocationService truckLocationService, ILogger<TruckHistoryController> logger)
        {
            _truckLocationService = truckLocationService;
            _logger = logger;
        }

        [HttpGet("history/{truckId}")]
        public async Task<IActionResult> GetTruckHistory(int truckId)
        {
            try
            {
                var history = await _truckLocationService.GetTruckHistoryAsync(truckId);
                if (history != null)
                {
                    return Ok(history);
                }
                else
                {
                    _logger.LogWarning($"No history found for truck with ID {truckId}.");
                    return NotFound(new { Message = "No history found for this truck." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting truck history for truck with ID {truckId}.");
                return StatusCode(500, new { Message = "Failed to retrieve truck history. Please try again later." });
            }
        }
    }
}
