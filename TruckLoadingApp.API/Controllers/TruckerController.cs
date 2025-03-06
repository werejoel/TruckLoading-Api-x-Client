using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Infrastructure.Data;
using TruckLoadingApp.API.Models.DTOs;
using System.Security.Claims;

namespace TruckLoadingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Trucker")]
    public class TruckerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TruckerController> _logger;

        public TruckerController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<TruckerController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Get trucks for the current trucker
        [HttpGet("trucks")]
        public async Task<ActionResult<IEnumerable<TruckDto>>> GetTruckerTrucks()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Fetching trucks for trucker with ID {UserId}", userId);

                var trucks = await _context.Trucks
                    .Include(t => t.TruckType)
                    .Where(t => t.OwnerId == userId)
                    .Select(t => new TruckDto
                    {
                        Id = t.Id,
                        NumberPlate = t.NumberPlate,
                        TruckType = t.TruckType != null ? t.TruckType.Name : "Unknown",
                        LoadCapacityWeight = t.LoadCapacityWeight,
                        IsApproved = t.IsApproved,
                        OperationalStatus = t.OperationalStatus.ToString()
                    })
                    .ToListAsync();

                return Ok(trucks);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving trucker trucks: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving trucks.", Error = ex.Message });
            }
        }

        // Get bookings for the current trucker
        [HttpGet("bookings")]
        public async Task<ActionResult<IEnumerable<object>>> GetBookings()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Fetching bookings for trucker with ID {UserId}", userId);

                // Get trucks owned by this trucker
                var truckIds = await _context.Trucks
                    .Where(t => t.OwnerId == userId)
                    .Select(t => t.Id)
                    .ToListAsync();

                if (truckIds.Count == 0)
                {
                    return Ok(new List<object>());
                }

                // Get bookings for these trucks
                var bookings = await _context.Bookings
                    .Include(b => b.Load)
                    .Include(b => b.Truck)
                    .Where(b => truckIds.Contains(b.TruckId))
                    .Select(b => new
                    {
                        b.Id,
                        b.LoadId,
                        Load = new
                        {
                            b.Load.Description,
                            b.Load.PickupAddress,
                            b.Load.DeliveryAddress,
                            b.Load.PickupDate,
                            b.Load.DeliveryDate,
                            b.Load.Weight
                        },
                        b.TruckId,
                        b.Status,
                        b.CreatedDate,
                        b.UpdatedDate,
                        b.AgreedPrice,
                        b.PricingType,
                        b.Currency
                    })
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving bookings: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving bookings.", Error = ex.Message });
            }
        }

        // Get a specific booking
        [HttpGet("bookings/{id}")]
        public async Task<ActionResult<object>> GetBookingById(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                // Get trucks owned by this trucker
                var truckIds = await _context.Trucks
                    .Where(t => t.OwnerId == userId)
                    .Select(t => t.Id)
                    .ToListAsync();

                // Get the booking
                var booking = await _context.Bookings
                    .Include(b => b.Load)
                    .Include(b => b.Truck)
                    .Where(b => b.Id == id && truckIds.Contains(b.TruckId))
                    .Select(b => new
                    {
                        b.Id,
                        b.LoadId,
                        Load = new
                        {
                            b.Load.Description,
                            b.Load.PickupAddress,
                            b.Load.DeliveryAddress,
                            b.Load.PickupDate,
                            b.Load.DeliveryDate,
                            b.Load.Weight,
                            b.Load.SpecialRequirements
                        },
                        b.TruckId,
                        b.Status,
                        b.CreatedDate,
                        b.UpdatedDate,
                        b.AgreedPrice,
                        b.PricingType,
                        b.Currency
                    })
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    return NotFound(new { Message = "Booking not found or you don't have access to it" });
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving booking: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving the booking.", Error = ex.Message });
            }
        }

        // Start transport for a booking
        [HttpPost("bookings/{id}/start")]
        public async Task<IActionResult> StartTransport(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                // Get trucks owned by this trucker
                var truckIds = await _context.Trucks
                    .Where(t => t.OwnerId == userId)
                    .Select(t => t.Id)
                    .ToListAsync();

                // Get the booking
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == id && truckIds.Contains(b.TruckId));

                if (booking == null)
                {
                    return NotFound(new { Message = "Booking not found or you don't have access to it" });
                }

                if (booking.Status != BookingStatusEnum.Accepted)
                {
                    return BadRequest(new { Message = "Booking must be in Accepted status to start transport" });
                }

                booking.Status = BookingStatusEnum.InTransit;
                booking.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Transport started successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error starting transport: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while starting transport.", Error = ex.Message });
            }
        }

        // Complete transport for a booking
        [HttpPost("bookings/{id}/complete")]
        public async Task<IActionResult> CompleteTransport(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                // Get trucks owned by this trucker
                var truckIds = await _context.Trucks
                    .Where(t => t.OwnerId == userId)
                    .Select(t => t.Id)
                    .ToListAsync();

                // Get the booking
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == id && truckIds.Contains(b.TruckId));

                if (booking == null)
                {
                    return NotFound(new { Message = "Booking not found or you don't have access to it" });
                }

                if (booking.Status != BookingStatusEnum.InTransit)
                {
                    return BadRequest(new { Message = "Booking must be in InTransit status to complete transport" });
                }

                booking.Status = BookingStatusEnum.Completed;
                booking.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Transport completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error completing transport: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while completing transport.", Error = ex.Message });
            }
        }

        // Report an issue with a booking
        [HttpPost("bookings/{id}/issue")]
        public async Task<IActionResult> ReportIssue(int id, [FromBody] IssueReportRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.IssueDetails))
                {
                    return BadRequest(new { Message = "Issue details are required" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                // Get trucks owned by this trucker
                var truckIds = await _context.Trucks
                    .Where(t => t.OwnerId == userId)
                    .Select(t => t.Id)
                    .ToListAsync();

                // Get the booking
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == id && truckIds.Contains(b.TruckId));

                if (booking == null)
                {
                    return NotFound(new { Message = "Booking not found or you don't have access to it" });
                }

                // Just update the timestamp to indicate something happened
                booking.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Issue reported for booking {BookingId} by user {UserId}: {IssueDetails}", 
                    booking.Id, userId, request.IssueDetails);

                return Ok(new { Message = "Issue reported successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reporting issue: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while reporting the issue.", Error = ex.Message });
            }
        }

        // Get available loads based on location and radius
        [HttpGet("loads/available")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableLoads(
            [FromQuery] decimal latitude,
            [FromQuery] decimal longitude,
            [FromQuery] int radius = 50,
            [FromQuery] decimal? maxWeight = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Fetching available loads for trucker with ID {UserId} at location ({Latitude}, {Longitude}) with radius {Radius}km",
                    userId, latitude, longitude, radius);

                // Find loads that are available (not assigned or in transit)
                var availableLoads = await _context.Loads
                    .Include(l => l.Shipper)
                    .Where(l => l.Status == Domain.Enums.LoadStatusEnum.Created || 
                                l.Status == Domain.Enums.LoadStatusEnum.Available)
                    .Where(l => l.PickupLatitude.HasValue && l.PickupLongitude.HasValue)
                    .ToListAsync();

                // Calculate distance and filter by radius
                var result = availableLoads
                    .Select(load => {
                        // Calculate distance using Haversine formula
                        double distance = CalculateDistance(
                            (double)latitude, 
                            (double)longitude, 
                            (double)load.PickupLatitude.Value, 
                            (double)load.PickupLongitude.Value);
                        
                        return new { Load = load, Distance = distance };
                    })
                    .Where(item => item.Distance <= radius)
                    .Where(item => !maxWeight.HasValue || item.Load.Weight <= maxWeight.Value)
                    .OrderBy(item => item.Distance)
                    .Select(item => new
                    {
                        id = item.Load.Id,
                        weight = item.Load.Weight,
                        dimensions = new
                        {
                            height = item.Load.Height,
                            width = item.Load.Width,
                            length = item.Load.Length
                        },
                        pickupDate = item.Load.PickupDate,
                        deliveryDate = item.Load.DeliveryDate,
                        pickupLocation = new
                        {
                            latitude = item.Load.PickupLatitude,
                            longitude = item.Load.PickupLongitude,
                            address = item.Load.PickupAddress
                        },
                        deliveryLocation = new
                        {
                            latitude = item.Load.DeliveryLatitude,
                            longitude = item.Load.DeliveryLongitude,
                            address = item.Load.DeliveryAddress
                        },
                        price = item.Load.Price,
                        currency = item.Load.Currency,
                        distance = item.Distance,
                        shipper = new
                        {
                            id = item.Load.ShipperId,
                            name = item.Load.Shipper != null ? $"{item.Load.Shipper.FirstName} {item.Load.Shipper.LastName}" : "Unknown"
                            // Add rating if available
                        }
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving available loads: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving available loads.", Error = ex.Message });
            }
        }

        // Helper method to calculate distance between two points using Haversine formula
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;
            
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }

    public class IssueReportRequest
    {
        public string IssueDetails { get; set; }
    }
} 