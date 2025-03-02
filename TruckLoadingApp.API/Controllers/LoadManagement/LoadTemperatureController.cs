using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.LoadManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoadTemperatureController : ControllerBase
    {
        private readonly ILoadTemperatureService _loadTemperatureService;
        private readonly ILogger<LoadTemperatureController> _logger;

        public LoadTemperatureController(
            ILoadTemperatureService loadTemperatureService,
            ILogger<LoadTemperatureController> logger)
        {
            _loadTemperatureService = loadTemperatureService;
            _logger = logger;
        }

        [HttpGet("load/{loadId}/readings")]
        public async Task<ActionResult<IEnumerable<TemperatureReading>>> GetLoadTemperatureReadings(long loadId)
        {
            try
            {
                var readings = await _loadTemperatureService.GetTemperatureReadingsAsync(loadId);
                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving temperature readings for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while retrieving load temperature readings");
            }
        }

        [HttpGet("reading/{id}")]
        public async Task<ActionResult<TemperatureReading>> GetTemperatureReadingById(long id)
        {
            try
            {
                var reading = await _loadTemperatureService.GetTemperatureReadingByIdAsync(id);
                if (reading == null)
                {
                    return NotFound();
                }
                return Ok(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving temperature reading with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the temperature reading");
            }
        }

        [HttpPost("reading")]
        [Authorize(Roles = "Admin,Manager,Driver")]
        public async Task<ActionResult<TemperatureReading>> RecordTemperatureReading([FromBody] TemperatureReading reading)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _loadTemperatureService.RecordTemperatureReadingAsync(
                    reading.LoadId,
                    reading.Temperature,
                    reading.Timestamp,
                    reading.DeviceId);

                if (!success)
                {
                    return BadRequest("Failed to record temperature reading");
                }

                var createdReading = await _loadTemperatureService.GetLatestTemperatureReadingAsync(reading.LoadId);
                return CreatedAtAction(nameof(GetTemperatureReadingById), new { id = createdReading.Id }, createdReading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating temperature reading for load with ID {LoadId}", reading.LoadId);
                return StatusCode(500, "An error occurred while creating the temperature reading");
            }
        }

        [HttpGet("requirement/{loadId}")]
        public async Task<ActionResult<LoadTemperatureRequirement>> GetTemperatureRequirement(long loadId)
        {
            try
            {
                var requirement = await _loadTemperatureService.GetTemperatureRequirementAsync(loadId);
                if (requirement == null)
                {
                    return NotFound();
                }
                return Ok(requirement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving temperature requirement for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while retrieving the temperature requirement");
            }
        }

        [HttpPost("requirement")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<LoadTemperatureRequirement>> CreateTemperatureRequirement([FromBody] LoadTemperatureRequirement requirement)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _loadTemperatureService.CreateTemperatureRequirementAsync(
                    requirement.LoadId,
                    requirement.MinTemperature,
                    requirement.MaxTemperature,
                    requirement.TemperatureUnit,
                    requirement.RequiresContinuousMonitoring,
                    requirement.MonitoringIntervalMinutes);

                return CreatedAtAction(nameof(GetTemperatureRequirement), new { loadId = result.LoadId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating temperature requirement for load with ID {LoadId}", requirement.LoadId);
                return StatusCode(500, "An error occurred while creating the temperature requirement");
            }
        }

        [HttpPut("requirement/{loadId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTemperatureRequirement(long loadId, [FromBody] LoadTemperatureRequirement requirement)
        {
            try
            {
                if (loadId != requirement.LoadId)
                {
                    return BadRequest("Load ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _loadTemperatureService.UpdateTemperatureRequirementAsync(
                    requirement.LoadId,
                    requirement.MinTemperature,
                    requirement.MaxTemperature,
                    requirement.TemperatureUnit,
                    requirement.RequiresContinuousMonitoring,
                    requirement.MonitoringIntervalMinutes);

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating temperature requirement for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while updating the temperature requirement");
            }
        }

        [HttpDelete("requirement/{loadId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTemperatureRequirement(long loadId)
        {
            try
            {
                var result = await _loadTemperatureService.DeleteTemperatureRequirementAsync(loadId);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting temperature requirement for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while deleting the temperature requirement");
            }
        }

        [HttpGet("load/{loadId}/latest")]
        public async Task<ActionResult<TemperatureReading>> GetLatestLoadTemperature(long loadId)
        {
            try
            {
                var reading = await _loadTemperatureService.GetLatestTemperatureReadingAsync(loadId);
                if (reading == null)
                {
                    return NotFound();
                }
                return Ok(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest temperature reading for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while retrieving the latest temperature reading");
            }
        }
    }
} 