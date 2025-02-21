using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Application.Services;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.API.Controllers
{
    /// <summary>
    /// Controller for managing trucks, including registration, route creation, waypoint management, location updates, and searching trucks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TruckController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TruckController> _logger;
        private readonly IMatchService _matchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TruckController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userManager">The UserManager for managing users.</param>
        /// <param name="logger">The ILogger for logging.</param>
        /// <param name="matchService">The service for matching loads with trucks.</param>
        public TruckController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<TruckController> logger,
            IMatchService matchService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _matchService = matchService;
        }

        /// <summary>
        /// Registers a new truck.
        /// </summary>
        /// <param name="model">The truck registration request containing truck details.</param>
        /// <returns>An IActionResult indicating the result of the truck registration.</returns>
        /// <response code="200">Truck registration successful, awaiting approval.</response>
        /// <response code="400">Invalid model state.</response>
        /// <response code="401">Unauthorized request.</response>
        [Authorize(Roles = "Company,Trucker")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterTruck([FromBody] TruckRegistrationRequest model)
        {
            _logger.LogInformation($"User authenticated: {User.Identity?.IsAuthenticated}");
            _logger.LogInformation($"User name: {User.Identity?.Name}");
            _logger.LogInformation($"User role: {User.FindFirst(ClaimTypes.Role)?.Value}");

            if (ModelState.IsValid)
            {
                // Get the current user's ID
                var user = await _userManager.GetUserAsync(User); // Get logged in user
                if (user == null || (user.UserType != UserType.Company && user.UserType != UserType.Trucker))
                {
                    return Unauthorized("Only truckers and companies can register trucks.");// Or handle the case where the user is not found
                }
                var truck = new Truck
                {
                    TruckTypeId = model.TruckTypeId,
                    OwnerId = user.Id,
                    NumberPlate = model.NumberPlate,
                    LoadCapacityWeight = model.LoadCapacityWeight,
                    LoadCapacityVolume = model.LoadCapacityVolume,
                    AvailabilityStartDate = model.AvailabilityStartDate,
                    AvailabilityEndDate = model.AvailabilityEndDate,
                    PreferredRoute = model.PreferredRoute,
                    DriverName = model.DriverName,
                    DriverContactInformation = model.DriverContactInformation,
                    InsuranceInformation = model.InsuranceInformation,
                    IsApproved = false, // Truck needs to be approved by admin
                    CreatedDate = DateTime.UtcNow
                };

                _context.Trucks.Add(truck);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Truck registration successful. Awaiting approval." });
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Creates a new route for a truck.
        /// </summary>
        /// <param name="model">The create route request containing route details.</param>
        /// <returns>An IActionResult indicating the result of the route creation.</returns>
        /// <response code="200">Route created successfully.</response>
        /// <response code="400">Invalid model state or truck does not belong to user.</response>
        /// <response code="401">Unauthorized request.</response>
        [HttpPost("createRoute")]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var truck = await _context.Trucks.FirstOrDefaultAsync(t => t.OwnerId == user.Id && t.Id == model.TruckId);

            if (truck == null)
            {
                return BadRequest(new { message = "Truck does not belong to user." });
            }

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var route = new Domain.Models.Route
            {
                TruckId = model.TruckId,
                RouteName = model.RouteName,
                CreatedDate = DateTime.UtcNow
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Route created successfully!", RouteId = route.Id });
        }

        /// <summary>
        /// Adds a new waypoint to a route.
        /// </summary>
        /// <param name="model">The add waypoint request containing waypoint details.</param>
        /// <returns>An IActionResult indicating the result of adding the waypoint.</returns>
        /// <response code="200">Waypoint added successfully.</response>
        /// <response code="400">Invalid model state.</response>
        /// <response code="404">Route not found.</response>
        [HttpPost("addWaypoint")]
        public async Task<IActionResult> AddWaypoint([FromBody] AddWaypointRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var route = await _context.Routes.FindAsync(model.RouteId);
            if (route == null)
            {
                return NotFound();
            }

            var waypoint = new Domain.Models.Waypoint
            {
                RouteId = model.RouteId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Location = geometryFactory.CreatePoint(new Coordinate((double)model.Longitude, (double)model.Latitude)),
                StopOrder = model.StopOrder,
                ArrivalDate = model.ArrivalDate
            };

            _context.Waypoints.Add(waypoint);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Waypoint added successfully!" });
        }

        /// <summary>
        /// Updates the location of a truck.
        /// </summary>
        /// <param name="model">The update location request containing truck location details.</param>
        /// <returns>An IActionResult indicating the result of the location update.</returns>
        /// <response code="200">Truck location updated successfully.</response>
        /// <response code="400">Invalid model state or truck does not belong to the authenticated user.</response>
        /// <response code="401">Unauthorized request.</response>
        [HttpPost("updatelocation")]
        [Authorize(Roles = "Trucker")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the authenticated user.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure the truck belongs to the authenticated user.
            var truck = await _context.Trucks.FirstOrDefaultAsync(t => t.Id == model.TruckId && t.OwnerId == user.Id);
            if (truck == null)
            {
                return BadRequest(new { message = "Truck does not belong to the authenticated user or does not exist." });
            }

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var truckLocation = new TruckLocation
            {
                TruckId = model.TruckId,
                CurrentLatitude = model.CurrentLatitude,
                CurrentLongitude = model.CurrentLongitude,
                CurrentLocation = geometryFactory.CreatePoint(new Coordinate((double)model.CurrentLongitude, (double)model.CurrentLatitude)),
                Timestamp = DateTime.UtcNow
            };

            _context.TruckLocations.Add(truckLocation);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Truck location updated successfully!" });
        }

        /// <summary>
        /// Retrieves a list of truck types.
        /// </summary>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of truck types per page.</param>
        /// <returns>An IActionResult containing a list of truck types.</returns>
        /// <response code="200">Returns a list of truck types.</response>
        [HttpGet("trucktypes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTruckTypes(int page = 1, int pageSize = 10)
        {
            var truckTypes = await _context.TruckTypes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return Ok(truckTypes);
        }

        /// <summary>
        /// Searches for trucks based on the provided criteria.
        /// </summary>
        /// <param name="model">The search trucks request containing search criteria.</param>
        /// <returns>An IActionResult containing a list of trucks that match the search criteria.</returns>
        /// <response code="200">Matching trucks found.</response>
        /// <response code="400">Invalid model state.</response>
        /// <response code="500">Error while matching trucks.</response>
        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> SearchTrucks([FromBody] SearchTrucksRequest model)
        {
            _logger.LogInformation("Attempting to search trucks");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in SearchTrucks");
                return BadRequest(ModelState);
            }

            try
            {
                var trucks = await _matchService.FindMatchingTrucks(
                    model.OriginLatitude,
                    model.OriginLongitude,
                    model.DestinationLatitude,
                    model.DestinationLongitude,
                    model.Weight,
                    model.Height,
                    model.Width,
                    model.Length,
                    model.PickupDate,
                    model.DeliveryDate
                );

                if (trucks == null || !trucks.Any())
                {
                    _logger.LogInformation("No matching trucks found");
                    return Ok(new { Message = "No matching trucks found", Trucks = Array.Empty<object>() });
                }

                _logger.LogInformation($"Found {trucks.Count()} matching trucks");
                return Ok(new { Message = "Matching trucks found", Trucks = trucks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while matching trucks");
                return StatusCode(500, new { Message = "An error occurred while matching trucks. Please try again." });
            }
        }
    }
}