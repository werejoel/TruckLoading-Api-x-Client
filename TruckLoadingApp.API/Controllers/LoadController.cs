using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Linq;
using System.Threading.Tasks;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Application.Services;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.API.Controllers
{
    /// <summary>
    /// Controller for managing loads, including posting new loads, matching trucks, and searching loads.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Shipper")]
    public class LoadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoadController> _logger;
        private readonly IMatchService _matchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userManager">The UserManager for managing users.</param>
        /// <param name="logger">The ILogger for logging.</param>
        /// <param name="matchService">The service for matching loads with trucks.</param>
        public LoadController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<LoadController> logger,
            IMatchService matchService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _matchService = matchService;
        }

        /// <summary>
        /// Posts a new load.
        /// </summary>
        /// <param name="model">The load posting request containing load details.</param>
        /// <returns>An IActionResult indicating the result of the load posting.</returns>
        /// <response code="200">Load posted successfully.</response>
        /// <response code="400">Invalid model state.</response>
        /// <response code="401">Unauthorized request - User email not found or user not found in database.</response>
        /// <response code="403">Forbidden: User is not a shipper.</response>
        /// <response code="500">Error saving load.</response>
        [HttpPost("post")]
        public async Task<IActionResult> PostLoad([FromBody] LoadPostRequest model)
        {
            _logger.LogInformation($"User.Identity.Name: {User.Identity?.Name}");
            _logger.LogInformation($"User Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            _logger.LogInformation($"Is User in Shipper Role: {User.IsInRole("Shipper")}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in PostLoad");
                return BadRequest(ModelState);
            }

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("Unauthorized request - User.Identity.Name is null.");
                return Unauthorized(new { Message = "User email not found in token." });
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                _logger.LogWarning($"Unauthorized request - User not found in database: {userEmail}");
                return Unauthorized(new { Message = "User not found in database." });
            }

            if (user.UserType != UserType.Shipper)
            {
                _logger.LogWarning($"Forbidden: User {user.Email} is not a shipper.");
                return Forbid();
            }

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var load = new Load
            {
                ShipperId = user.Id,
                OriginLatitude = model.OriginLatitude,
                OriginLongitude = model.OriginLongitude,
                DestinationLatitude = model.DestinationLatitude,
                DestinationLongitude = model.DestinationLongitude,
                OriginLocation = geometryFactory.CreatePoint(new Coordinate((double)model.OriginLongitude, (double)model.OriginLatitude)),
                DestinationLocation = geometryFactory.CreatePoint(new Coordinate((double)model.DestinationLongitude, (double)model.DestinationLatitude)),
                Weight = model.Weight,
                Height = model.Height,
                Width = model.Width,
                Length = model.Length,
                Description = model.Description,
                PickupDate = model.PickupDate,
                DeliveryDate = model.DeliveryDate,
                CreatedDate = DateTime.UtcNow
            };

            try
            {
                _context.Loads.Add(load);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Load posted successfully by {user.Email}");
                return Ok(new { Message = "Load posted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving load for user {user.Email}");
                return StatusCode(500, new { Message = "Error saving load. Please try again." });
            }
        }

        /// <summary>
        /// Matches a truck for a given load based on the provided criteria.
        /// </summary>
        /// <param name="model">The match load request containing matching criteria.</param>
        /// <returns>An IActionResult containing a list of matching trucks.</returns>
        /// <response code="200">Matching trucks found.</response>
        /// <response code="400">Invalid model state.</response>
        /// <response code="500">Error while matching trucks.</response>
        [HttpPost("match")]
        public async Task<IActionResult> MatchTruck([FromBody] MatchLoadRequest model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in MatchTruck");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Attempting to match a truck for load");

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

        /// <summary>
        /// Searches for loads based on the provided criteria.
        /// </summary>
        /// <param name="model">The search loads request containing search criteria.</param>
        /// <returns>An IActionResult containing a list of loads that match the search criteria.</returns>
        /// <response code="200">Loads found.</response>
        /// <response code="400">Invalid model state.</response>
        /// <response code="500">Error searching loads.</response>
        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> SearchLoads([FromBody] SearchLoadsRequest model)
        {
            _logger.LogInformation("Attempting to search loads");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in SearchLoads");
                return BadRequest(ModelState);
            }

            try
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var originPoint = geometryFactory.CreatePoint(new Coordinate((double)model.OriginLongitude, (double)model.OriginLatitude));
                var destinationPoint = geometryFactory.CreatePoint(new Coordinate((double)model.DestinationLongitude, (double)model.DestinationLatitude));
                var maxDistanceInMeters = 200 * 1000; // 200 km

                var loads = await _context.Loads
                    .Where(l => l.OriginLocation.Distance(originPoint) <= maxDistanceInMeters &&
                               l.DestinationLocation.Distance(destinationPoint) <= maxDistanceInMeters &&
                               l.Weight <= model.MaxWeight &&
                               l.PickupDate >= model.PickupDate &&
                               l.DeliveryDate <= model.DeliveryDate)
                    .ToListAsync();

                return Ok(new { Message = "Loads found", Loads = loads });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching loads");
                return StatusCode(500, new { Message = "An error occurred while searching loads. Please try again." });
            }
        }
    }
}