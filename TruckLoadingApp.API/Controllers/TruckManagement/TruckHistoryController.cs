using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.TruckManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TruckHistoryController : ControllerBase
    {
        private readonly ITruckHistoryService _truckHistoryService;
        private readonly ILogger<TruckHistoryController> _logger;

        public TruckHistoryController(
            ITruckHistoryService truckHistoryService,
            ILogger<TruckHistoryController> logger)
        {
            _truckHistoryService = truckHistoryService;
            _logger = logger;
        }

        [HttpGet("truck/{truckId}")]
        public async Task<ActionResult<IEnumerable<TruckHistory>>> GetTruckHistory(long truckId)
        {
            try
            {
                var history = await _truckHistoryService.GetTruckHistoryAsync(truckId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for truck with ID {TruckId}", truckId);
                return StatusCode(500, "An error occurred while retrieving truck history");
            }
        }
    }
} 