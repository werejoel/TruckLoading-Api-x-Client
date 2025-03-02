using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.API.Services;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.TruckManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TruckLocationController : ControllerBase
    {
        private readonly ITruckLocationService _truckLocationService;
        private readonly ILogger<TruckLocationController> _logger;

        public TruckLocationController(
            ITruckLocationService truckLocationService,
            ILogger<TruckLocationController> logger)
        {
            _truckLocationService = truckLocationService;
            _logger = logger;
        }

        [HttpGet("truck/{truckId}")]
        public async Task<ActionResult<TruckLocation>> GetCurrentTruckLocation(long truckId)
        {
            try
            {
                var location = await _truckLocationService.GetCurrentTruckLocationAsync(truckId);
                if (location == null)
                {
                    return NotFound();
                }
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving location for truck with ID {TruckId}", truckId);
                return StatusCode(500, "An error occurred while retrieving truck location");
            }
        }
    }
} 