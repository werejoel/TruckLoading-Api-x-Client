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
    public class DriverRoutePreferenceController : ControllerBase
    {
        private readonly IDriverRoutePreferenceService _driverRoutePreferenceService;
        private readonly ILogger<DriverRoutePreferenceController> _logger;

        public DriverRoutePreferenceController(
            IDriverRoutePreferenceService driverRoutePreferenceService,
            ILogger<DriverRoutePreferenceController> logger)
        {
            _driverRoutePreferenceService = driverRoutePreferenceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<DriverRoutePreference>> SetRoutePreferences([FromBody] DriverRoutePreference preferences)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverRoutePreferenceService.SetRoutePreferencesAsync(preferences);
                return CreatedAtAction(nameof(GetRoutePreferences), new { driverId = result.DriverId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting route preferences");
                return StatusCode(500, "An error occurred while setting the route preferences");
            }
        }

        [HttpGet("drivers/{driverId}")]
        public async Task<ActionResult<DriverRoutePreference>> GetRoutePreferences(long driverId)
        {
            try
            {
                var preferences = await _driverRoutePreferenceService.GetRoutePreferencesAsync(driverId);
                if (preferences == null)
                {
                    return NotFound();
                }
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving route preferences for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while retrieving the route preferences");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoutePreferences([FromBody] DriverRoutePreference preferences)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverRoutePreferenceService.UpdateRoutePreferencesAsync(preferences);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating route preferences for driver {DriverId}", preferences.DriverId);
                return StatusCode(500, "An error occurred while updating the route preferences");
            }
        }

        [HttpGet("compatible-drivers")]
        public async Task<ActionResult<IEnumerable<Driver>>> FindDriversByRoutePreferences([FromBody] RouteRequirements requirements)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var drivers = await _driverRoutePreferenceService.FindDriversByRoutePreferencesAsync(requirements);
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding drivers by route preferences");
                return StatusCode(500, "An error occurred while finding compatible drivers");
            }
        }
    }
} 