using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.API.Models.Responses;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,TruckOwner")]
    public class TruckRouteController : ControllerBase
    {
        private readonly ITruckRouteService _truckRouteService;
        private readonly ITruckService _truckService;
        private readonly ILogger<TruckRouteController> _logger;

        public TruckRouteController(
            ITruckRouteService truckRouteService,
            ITruckService truckService,
            ILogger<TruckRouteController> logger)
        {
            _truckRouteService = truckRouteService;
            _truckService = truckService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TruckRouteResponse>>> GetAllRoutes()
        {
            var routes = await _truckRouteService.GetAllRoutesAsync();
            var responses = routes.Select(MapToTruckRouteResponse);
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TruckRouteResponse>> GetRouteById(long id)
        {
            var route = await _truckRouteService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            var response = MapToTruckRouteResponse(route);
            return Ok(response);
        }

        [HttpGet("truck/{truckId}")]
        public async Task<ActionResult<IEnumerable<TruckRouteResponse>>> GetRoutesByTruckId(long truckId)
        {
            var truck = await _truckService.GetTruckByIdAsync(truckId);
            if (truck == null)
                return NotFound("Truck not found");

            var routes = await _truckRouteService.GetRoutesByTruckIdAsync(truckId);
            var responses = routes.Select(MapToTruckRouteResponse);
            return Ok(responses);
        }

        [HttpPost]
        public async Task<ActionResult<TruckRouteResponse>> CreateRoute([FromBody] TruckRouteCreateRequest request)
        {
            var truck = await _truckService.GetTruckByIdAsync(request.TruckId);
            if (truck == null)
                return NotFound("Truck not found");

            var route = new TruckRoute
            {
                TruckId = (int)request.TruckId,
                RouteName = request.RouteName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive,
                IsRecurring = request.IsRecurring,
                RecurrencePattern = request.RecurrencePattern,
                BasePricePerKm = request.BasePricePerKm,
                BasePricePerKg = request.BasePricePerKg,
                Currency = request.Currency
            };

            var createdRoute = await _truckRouteService.CreateRouteAsync(route);

            // Add waypoints if any
            if (request.Waypoints != null && request.Waypoints.Any())
            {
                foreach (var waypointRequest in request.Waypoints)
                {
                    var waypoint = new TruckRouteWaypoint
                    {
                        TruckRouteId = createdRoute.Id,
                        SequenceNumber = waypointRequest.SequenceNumber,
                        Address = waypointRequest.Address,
                        Latitude = waypointRequest.Latitude,
                        Longitude = waypointRequest.Longitude,
                        EstimatedArrivalTime = waypointRequest.EstimatedArrivalTime,
                        StopDurationMinutes = waypointRequest.StopDurationMinutes,
                        Notes = waypointRequest.Notes
                    };

                    await _truckRouteService.AddWaypointAsync(waypoint);
                }
            }

            // Reload the route with waypoints
            var routeWithWaypoints = await _truckRouteService.GetRouteByIdAsync(createdRoute.Id);
            var response = MapToTruckRouteResponse(routeWithWaypoints!);

            return CreatedAtAction(nameof(GetRouteById), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(long id, [FromBody] TruckRouteCreateRequest request)
        {
            var existingRoute = await _truckRouteService.GetRouteByIdAsync(id);
            if (existingRoute == null)
                return NotFound();

            var truck = await _truckService.GetTruckByIdAsync(request.TruckId);
            if (truck == null)
                return NotFound("Truck not found");

            existingRoute.TruckId = (int)request.TruckId;
            existingRoute.RouteName = request.RouteName;
            existingRoute.StartDate = request.StartDate;
            existingRoute.EndDate = request.EndDate;
            existingRoute.IsActive = request.IsActive;
            existingRoute.IsRecurring = request.IsRecurring;
            existingRoute.RecurrencePattern = request.RecurrencePattern;
            existingRoute.BasePricePerKm = request.BasePricePerKm;
            existingRoute.BasePricePerKg = request.BasePricePerKg;
            existingRoute.Currency = request.Currency;

            var result = await _truckRouteService.UpdateRouteAsync(existingRoute);
            if (!result)
                return StatusCode(500, "Failed to update route");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(long id)
        {
            var route = await _truckRouteService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            var result = await _truckRouteService.DeleteRouteAsync(id);
            if (!result)
                return StatusCode(500, "Failed to delete route");

            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateRoute(long id)
        {
            var route = await _truckRouteService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            var result = await _truckRouteService.ActivateRouteAsync(id);
            if (!result)
                return StatusCode(500, "Failed to activate route");

            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateRoute(long id)
        {
            var route = await _truckRouteService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            var result = await _truckRouteService.DeactivateRouteAsync(id);
            if (!result)
                return StatusCode(500, "Failed to deactivate route");

            return NoContent();
        }

        [HttpGet("{routeId}/waypoints")]
        public async Task<ActionResult<IEnumerable<WaypointResponse>>> GetWaypoints(long routeId)
        {
            var route = await _truckRouteService.GetRouteByIdAsync(routeId);
            if (route == null)
                return NotFound();

            var waypoints = await _truckRouteService.GetWaypointsByRouteIdAsync(routeId);
            var responses = waypoints.Select(MapToWaypointResponse);
            return Ok(responses);
        }

        [HttpPost("{routeId}/waypoints")]
        public async Task<ActionResult<WaypointResponse>> AddWaypoint(long routeId, [FromBody] WaypointCreateRequest request)
        {
            var route = await _truckRouteService.GetRouteByIdAsync(routeId);
            if (route == null)
                return NotFound();

            var waypoint = new TruckRouteWaypoint
            {
                TruckRouteId = routeId,
                SequenceNumber = request.SequenceNumber,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                EstimatedArrivalTime = request.EstimatedArrivalTime,
                StopDurationMinutes = request.StopDurationMinutes,
                Notes = request.Notes
            };

            var createdWaypoint = await _truckRouteService.AddWaypointAsync(waypoint);
            var response = MapToWaypointResponse(createdWaypoint);

            return CreatedAtAction(nameof(GetWaypoints), new { routeId }, response);
        }

        [HttpPut("waypoints/{waypointId}")]
        public async Task<IActionResult> UpdateWaypoint(long waypointId, [FromBody] WaypointCreateRequest request)
        {
            var waypoints = await _truckRouteService.GetWaypointsByRouteIdAsync(request.SequenceNumber);
            var existingWaypoint = waypoints.FirstOrDefault(w => w.Id == waypointId);
            
            if (existingWaypoint == null)
                return NotFound();

            existingWaypoint.SequenceNumber = request.SequenceNumber;
            existingWaypoint.Address = request.Address;
            existingWaypoint.Latitude = request.Latitude;
            existingWaypoint.Longitude = request.Longitude;
            existingWaypoint.EstimatedArrivalTime = request.EstimatedArrivalTime;
            existingWaypoint.StopDurationMinutes = request.StopDurationMinutes;
            existingWaypoint.Notes = request.Notes;

            var result = await _truckRouteService.UpdateWaypointAsync(existingWaypoint);
            if (!result)
                return StatusCode(500, "Failed to update waypoint");

            return NoContent();
        }

        [HttpDelete("waypoints/{waypointId}")]
        public async Task<IActionResult> DeleteWaypoint(long waypointId)
        {
            var result = await _truckRouteService.DeleteWaypointAsync(waypointId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        #region Mapping Methods

        private TruckRouteResponse MapToTruckRouteResponse(TruckRoute route)
        {
            return new TruckRouteResponse
            {
                Id = route.Id,
                TruckId = route.TruckId,
                TruckRegistrationNumber = route.Truck?.RegistrationNumber ?? string.Empty,
                RouteName = route.RouteName,
                StartDate = route.StartDate,
                EndDate = route.EndDate,
                IsActive = route.IsActive,
                IsRecurring = route.IsRecurring,
                RecurrencePattern = route.RecurrencePattern,
                BasePricePerKm = route.BasePricePerKm,
                BasePricePerKg = route.BasePricePerKg,
                Currency = route.Currency,
                CreatedDate = route.CreatedDate,
                UpdatedDate = route.UpdatedDate,
                Waypoints = route.Waypoints?.Select(MapToWaypointResponse).ToList() ?? new List<WaypointResponse>()
            };
        }

        private WaypointResponse MapToWaypointResponse(TruckRouteWaypoint waypoint)
        {
            return new WaypointResponse
            {
                Id = waypoint.Id,
                TruckRouteId = waypoint.TruckRouteId,
                SequenceNumber = waypoint.SequenceNumber,
                Address = waypoint.Address,
                Latitude = waypoint.Latitude,
                Longitude = waypoint.Longitude,
                EstimatedArrivalTime = waypoint.EstimatedArrivalTime,
                StopDurationMinutes = waypoint.StopDurationMinutes,
                Notes = waypoint.Notes,
                CreatedDate = waypoint.CreatedDate,
                UpdatedDate = waypoint.UpdatedDate
            };
        }

        #endregion
    }
} 