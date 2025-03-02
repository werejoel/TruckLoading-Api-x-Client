using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.LoadManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoadController : ControllerBase
    {
        private readonly ILoadService _loadService;
        private readonly ILogger<LoadController> _logger;

        public LoadController(
            ILoadService loadService,
            ILogger<LoadController> logger)
        {
            _loadService = loadService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Load>>> GetAllLoads()
        {
            try
            {
                var loads = await _loadService.GetAllLoadsAsync();
                return Ok(loads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all loads");
                return StatusCode(500, "An error occurred while retrieving loads");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Load>> GetLoadById(long id)
        {
            try
            {
                var load = await _loadService.GetLoadByIdAsync(id);
                if (load == null)
                {
                    return NotFound();
                }
                return Ok(load);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving load with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the load");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Load>> CreateLoad([FromBody] Load load)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _loadService.CreateLoadAsync(load);
                return CreatedAtAction(nameof(GetLoadById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating load");
                return StatusCode(500, "An error occurred while creating the load");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateLoad(long id, [FromBody] Load load)
        {
            try
            {
                if (id != load.Id)
                {
                    return BadRequest("Load ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _loadService.UpdateLoadAsync(load);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating load with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the load");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLoad(long id)
        {
            try
            {
                var result = await _loadService.DeleteLoadAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting load with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the load");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Load>>> GetLoadsByStatus(string status)
        {
            try
            {
                var loads = await _loadService.GetLoadsByStatusAsync(status);
                return Ok(loads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving loads with status {Status}", status);
                return StatusCode(500, "An error occurred while retrieving loads by status");
            }
        }

        [HttpPost("{loadId}/assign-truck/{truckId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignTruckToLoad(long loadId, long truckId)
        {
            try
            {
                var result = await _loadService.AssignTruckToLoadAsync(loadId, truckId);
                if (!result)
                {
                    return BadRequest("Failed to assign truck to load");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning truck {TruckId} to load {LoadId}", truckId, loadId);
                return StatusCode(500, "An error occurred while assigning the truck to the load");
            }
        }
    }
} 