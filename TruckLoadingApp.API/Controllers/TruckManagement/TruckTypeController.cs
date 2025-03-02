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
    public class TruckTypeController : ControllerBase
    {
        private readonly ITruckTypeService _truckTypeService;
        private readonly ILogger<TruckTypeController> _logger;

        public TruckTypeController(
            ITruckTypeService truckTypeService,
            ILogger<TruckTypeController> logger)
        {
            _truckTypeService = truckTypeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TruckType>>> GetAllTruckTypes()
        {
            try
            {
                var truckTypes = await _truckTypeService.GetAllTruckTypesAsync();
                return Ok(truckTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all truck types");
                return StatusCode(500, "An error occurred while retrieving truck types");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TruckType>> GetTruckTypeById(int id)
        {
            try
            {
                var truckType = await _truckTypeService.GetTruckTypeByIdAsync(id);
                if (truckType == null)
                {
                    return NotFound();
                }
                return Ok(truckType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck type with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the truck type");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TruckType>> CreateTruckType([FromBody] TruckType truckType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _truckTypeService.CreateTruckTypeAsync(truckType);
                return CreatedAtAction(nameof(GetTruckTypeById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating truck type");
                return StatusCode(500, "An error occurred while creating the truck type");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTruckType(int id, [FromBody] TruckType truckType)
        {
            try
            {
                if (id != truckType.Id)
                {
                    return BadRequest("Truck type ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _truckTypeService.UpdateTruckTypeAsync(truckType);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating truck type with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the truck type");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTruckType(int id)
        {
            try
            {
                var result = await _truckTypeService.DeleteTruckTypeAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting truck type with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the truck type");
            }
        }
    }
} 