using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TruckLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TruckLocationController> _logger;

        public TruckLocationController(ApplicationDbContext context, ILogger<TruckLocationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("history/{truckId}")]
        public async Task<IActionResult> GetTruckHistory(int truckId)
        {
            try
            {
                var history = await _context.TruckLocationHistories
                    .Where(tl => tl.TruckId == truckId)
                    .ToListAsync();

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting truck location history for truck with ID {truckId}.");
                return StatusCode(500, new { Message = "Failed to retrieve truck location history. Please try again later." });
            }
        }
    }
}
