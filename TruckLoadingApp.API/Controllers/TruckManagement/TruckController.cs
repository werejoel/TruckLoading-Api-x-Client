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
    [Route("api/truck-management")]
    [Authorize]
    public class TruckController : ControllerBase
    {
        private readonly ITruckService _truckService;
        private readonly ILogger<TruckController> _logger;

        public TruckController(
            ITruckService truckService,
            ILogger<TruckController> logger)
        {
            _truckService = truckService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Truck>>> GetAllTrucks()
        {
            try
            {
                var trucks = await _truckService.GetAllTrucksAsync();
                return Ok(trucks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all trucks");
                return StatusCode(500, "An error occurred while retrieving trucks");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Truck>> GetTruckById(long id)
        {
            try
            {
                var truck = await _truckService.GetTruckByIdAsync(id);
                if (truck == null)
                {
                    return NotFound();
                }
                return Ok(truck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the truck");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Truck>> CreateTruck([FromBody] Truck truck)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _truckService.CreateTruckAsync(truck);
                return CreatedAtAction(nameof(GetTruckById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating truck");
                return StatusCode(500, "An error occurred while creating the truck");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTruck(long id, [FromBody] Truck truck)
        {
            try
            {
                if (id != truck.Id)
                {
                    return BadRequest("Truck ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _truckService.UpdateTruckAsync(truck);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating truck with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the truck");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTruck(long id)
        {
            try
            {
                var result = await _truckService.DeleteTruckAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting truck with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the truck");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Truck>>> GetAvailableTrucks()
        {
            try
            {
                var trucks = await _truckService.GetAvailableTrucksAsync();
                return Ok(trucks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available trucks");
                return StatusCode(500, "An error occurred while retrieving available trucks");
            }
        }
    }
} 