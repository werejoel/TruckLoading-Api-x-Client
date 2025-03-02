using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.API.Models.Responses;

namespace TruckLoadingApp.API.Controllers.LoadManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoadController : ControllerBase
    {
        private readonly ILoadService _loadService;
        private readonly ITruckRouteService _truckRouteService;
        private readonly ILogger<LoadController> _logger;

        public LoadController(
            ILoadService loadService,
            ITruckRouteService truckRouteService,
            ILogger<LoadController> logger)
        {
            _loadService = loadService;
            _truckRouteService = truckRouteService;
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

        [HttpGet("{id}/matching-trucks")]
        public async Task<ActionResult<IEnumerable<TruckMatchResponse>>> GetMatchingTrucks(long id, [FromQuery] double maxDistanceKm = 50)
        {
            try
            {
                var load = await _loadService.GetLoadByIdAsync(id);
                if (load == null)
                {
                    return NotFound();
                }

                var matchingTrucks = await _truckRouteService.FindTrucksForLoadAsync(id, maxDistanceKm);
                var response = matchingTrucks.Select(t => new TruckMatchResponse
                {
                    Id = t.Id,
                    RegistrationNumber = t.RegistrationNumber,
                    TruckTypeId = t.TruckTypeId,
                    TruckTypeName = t.TruckType?.Name ?? string.Empty,
                    LoadCapacityWeight = t.LoadCapacityWeight,
                    AvailableCapacityWeight = t.AvailableCapacityWeight,
                    Height = t.Height,
                    Width = t.Width,
                    Length = t.Length,
                    VolumeCapacity = t.VolumeCapacity,
                    HasRefrigeration = t.HasRefrigeration,
                    HasLiftgate = t.HasLiftgate,
                    HasLoadingRamp = t.HasLoadingRamp,
                    CanTransportHazardousMaterials = t.CanTransportHazardousMaterials,
                    HazardousMaterialsClasses = t.HazardousMaterialsClasses,
                    DriverName = t.AssignedDriver?.FullName,
                    CompanyName = t.CompanyName,
                    CompanyRating = t.CompanyRating,
                    DistanceToPickup = (double)t.DistanceToPickup,
                    EstimatedTimeToPickup = CalculateEstimatedTime(t.DistanceToPickup)
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving matching trucks for load with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving matching trucks");
            }
        }

        private TimeSpan? CalculateEstimatedTime(decimal distanceKm)
        {
            if (distanceKm <= 0)
            {
                return null;
            }

            // Assuming average speed of 60 km/h
            const double averageSpeedKmh = 60.0;
            double hours = (double)distanceKm / averageSpeedKmh;
            
            return TimeSpan.FromHours(hours);
        }
    }

    public class TruckMatchResponse
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public int TruckTypeId { get; set; }
        public string TruckTypeName { get; set; } = string.Empty;
        public decimal LoadCapacityWeight { get; set; }
        public decimal AvailableCapacityWeight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal VolumeCapacity { get; set; }
        public bool HasRefrigeration { get; set; }
        public bool HasLiftgate { get; set; }
        public bool HasLoadingRamp { get; set; }
        public bool CanTransportHazardousMaterials { get; set; }
        public string? HazardousMaterialsClasses { get; set; }
        public string? DriverName { get; set; }
        public string? CompanyName { get; set; }
        public double? CompanyRating { get; set; }
        public double DistanceToPickup { get; set; }
        public TimeSpan? EstimatedTimeToPickup { get; set; }
    }
} 