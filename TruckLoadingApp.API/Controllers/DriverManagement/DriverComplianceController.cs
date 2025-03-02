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
    public class DriverComplianceController : ControllerBase
    {
        private readonly IDriverComplianceService _driverComplianceService;
        private readonly ILogger<DriverComplianceController> _logger;

        public DriverComplianceController(
            IDriverComplianceService driverComplianceService,
            ILogger<DriverComplianceController> logger)
        {
            _driverComplianceService = driverComplianceService;
            _logger = logger;
        }

        [HttpPost("rest-periods")]
        public async Task<ActionResult<DriverRestPeriod>> RecordRestPeriod([FromBody] DriverRestPeriod restPeriod)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverComplianceService.RecordRestPeriodAsync(restPeriod);
                return CreatedAtAction(nameof(GetRestPeriodById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording rest period");
                return StatusCode(500, "An error occurred while recording the rest period");
            }
        }

        [HttpGet("rest-periods/{id}")]
        public async Task<ActionResult<DriverRestPeriod>> GetRestPeriodById(long id)
        {
            try
            {
                var restPeriod = await _driverComplianceService.GetRestPeriodByIdAsync(id);
                if (restPeriod == null)
                {
                    return NotFound();
                }
                return Ok(restPeriod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rest period with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the rest period");
            }
        }

        [HttpGet("drivers/{driverId}/rest-periods")]
        public async Task<ActionResult<IEnumerable<DriverRestPeriod>>> GetDriverRestPeriods(
            long driverId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today.AddDays(30);
                
                var restPeriods = await _driverComplianceService.GetDriverRestPeriodsAsync(driverId, start, end);
                return Ok(restPeriods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rest periods for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while retrieving the driver's rest periods");
            }
        }

        [HttpPut("rest-periods/{id}")]
        public async Task<IActionResult> UpdateRestPeriod(long id, [FromBody] DriverRestPeriod restPeriod)
        {
            try
            {
                if (id != restPeriod.Id)
                {
                    return BadRequest("Rest period ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverComplianceService.UpdateRestPeriodAsync(restPeriod);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rest period with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the rest period");
            }
        }

        [HttpDelete("rest-periods/{id}")]
        public async Task<IActionResult> DeleteRestPeriod(long id)
        {
            try
            {
                var result = await _driverComplianceService.DeleteRestPeriodAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rest period with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the rest period");
            }
        }

        [HttpGet("drivers/{driverId}/compliance-check")]
        public async Task<ActionResult<Application.Services.DriverManagement.Interfaces.RestComplianceStatus>> CheckRestCompliance(long driverId)
        {
            try
            {
                var complianceStatus = await _driverComplianceService.CheckRestComplianceAsync(driverId);
                return Ok(complianceStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking rest compliance for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while checking rest compliance");
            }
        }

        [HttpGet("drivers/{driverId}/next-required-rest")]
        public async Task<ActionResult<DateTime>> GetNextRequiredRestTime(long driverId)
        {
            try
            {
                var nextRestTime = await _driverComplianceService.GetNextRequiredRestTimeAsync(driverId);
                return Ok(nextRestTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next required rest time for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while getting the next required rest time");
            }
        }

        [HttpGet("drivers/{driverId}/availability")]
        public async Task<ActionResult<bool>> IsDriverAvailable(
            long driverId, 
            [FromQuery] DateTime startTime, 
            [FromQuery] DateTime endTime)
        {
            try
            {
                var isAvailable = await _driverComplianceService.IsDriverAvailableAsync(driverId, startTime, endTime);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while checking driver availability");
            }
        }

        [HttpGet("available-drivers")]
        public async Task<ActionResult<IEnumerable<DriverSchedule>>> GetAvailableDriversForTimeSlot(
            [FromQuery] DateTime startTime, 
            [FromQuery] DateTime endTime)
        {
            try
            {
                var availableDrivers = await _driverComplianceService.GetAvailableDriversForTimeSlotAsync(startTime, endTime);
                return Ok(availableDrivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available drivers for time slot");
                return StatusCode(500, "An error occurred while getting available drivers");
            }
        }
    }
} 