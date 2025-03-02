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
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly ILogger<DriverController> _logger;

        public DriverController(
            IDriverService driverService,
            ILogger<DriverController> logger)
        {
            _driverService = driverService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetAllDrivers()
        {
            try
            {
                var drivers = await _driverService.GetAllDriversAsync();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all drivers");
                return StatusCode(500, "An error occurred while retrieving drivers");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriverById(long id)
        {
            try
            {
                var driver = await _driverService.GetDriverByIdAsync(id);
                if (driver == null)
                {
                    return NotFound();
                }
                return Ok(driver);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving driver with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the driver");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Driver>> CreateDriver([FromBody] Driver driver)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverService.CreateDriverAsync(driver);
                return CreatedAtAction(nameof(GetDriverById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating driver");
                return StatusCode(500, "An error occurred while creating the driver");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateDriver(long id, [FromBody] Driver driver)
        {
            try
            {
                if (id != driver.Id)
                {
                    return BadRequest("Driver ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverService.UpdateDriverAsync(driver);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the driver");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDriver(long id)
        {
            try
            {
                var result = await _driverService.DeleteDriverAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting driver with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the driver");
            }
        }

        [HttpPost("{driverId}/assign-truck/{truckId}")]
        public async Task<IActionResult> AssignDriverToTruck(long driverId, long truckId)
        {
            try
            {
                var result = await _driverService.AssignDriverToTruckAsync(driverId, truckId);
                if (!result)
                {
                    return BadRequest("Failed to assign driver to truck");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning driver {DriverId} to truck {TruckId}", driverId, truckId);
                return StatusCode(500, "An error occurred while assigning the driver to the truck");
            }
        }

        [HttpPost("{driverId}/unassign")]
        public async Task<IActionResult> UnassignDriver(long driverId)
        {
            try
            {
                var result = await _driverService.UnassignDriverAsync(driverId);
                if (!result)
                {
                    return BadRequest("Failed to unassign driver");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unassigning driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while unassigning the driver");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Driver>>> GetAvailableDrivers()
        {
            try
            {
                var drivers = await _driverService.GetAvailableDriversAsync();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available drivers");
                return StatusCode(500, "An error occurred while retrieving available drivers");
            }
        }
    }
} 