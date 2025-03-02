using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.DriverManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverPerformanceController : ControllerBase
    {
        private readonly IDriverPerformanceService _driverPerformanceService;
        private readonly ILogger<DriverPerformanceController> _logger;

        public DriverPerformanceController(
            IDriverPerformanceService driverPerformanceService,
            ILogger<DriverPerformanceController> logger)
        {
            _driverPerformanceService = driverPerformanceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<DriverPerformance>> RecordPerformance([FromBody] DriverPerformance performance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverPerformanceService.RecordPerformanceAsync(performance);
                return CreatedAtAction(nameof(GetPerformanceById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording driver performance");
                return StatusCode(500, "An error occurred while recording the driver performance");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverPerformance>> GetPerformanceById(long id)
        {
            try
            {
                var performance = await _driverPerformanceService.GetPerformanceByIdAsync(id);
                if (performance == null)
                {
                    return NotFound();
                }
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving performance with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the performance record");
            }
        }

        [HttpGet("drivers/{driverId}/history")]
        public async Task<ActionResult<IEnumerable<DriverPerformance>>> GetDriverPerformanceHistory(
            long driverId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;
                
                var history = await _driverPerformanceService.GetDriverPerformanceHistoryAsync(driverId, start, end);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving performance history for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while retrieving the driver's performance history");
            }
        }

        [HttpGet("drivers/{driverId}/rating")]
        public async Task<ActionResult<decimal>> CalculateDriverRating(long driverId)
        {
            try
            {
                var rating = await _driverPerformanceService.CalculateDriverRatingAsync(driverId);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating rating for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while calculating the driver's rating");
            }
        }

        [HttpGet("drivers/{driverId}/metrics")]
        public async Task<ActionResult<PerformanceMetrics>> GetDriverMetrics(
            long driverId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;
                
                var metrics = await _driverPerformanceService.GetDriverMetricsAsync(driverId, start, end);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving metrics for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while retrieving the driver's metrics");
            }
        }

        [HttpPost("safety-score")]
        public ActionResult<decimal> CalculateSafetyScore([FromBody] List<DriverPerformance> performances)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var score = _driverPerformanceService.CalculateSafetyScore(performances);
                return Ok(score);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating safety score");
                return StatusCode(500, "An error occurred while calculating the safety score");
            }
        }
    }
} 