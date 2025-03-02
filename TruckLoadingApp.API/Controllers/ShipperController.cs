using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.API.Models.Responses;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Shipper")]
    public class ShipperController : ControllerBase
    {
        private readonly IShipperService _shipperService;
        private readonly ILogger<ShipperController> _logger;

        public ShipperController(
            IShipperService shipperService,
            ILogger<ShipperController> logger)
        {
            _shipperService = shipperService;
            _logger = logger;
        }

        // Helper method to get the current user's ID
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                throw new UnauthorizedAccessException("User ID not found in claims");
        }

        #region Load Management

        [HttpPost("loads")]
        public async Task<ActionResult<LoadResponse>> CreateLoad([FromBody] LoadCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var load = new Load
                {
                    Weight = request.Weight,
                    Height = request.Height,
                    Width = request.Width,
                    Length = request.Length,
                    Description = request.Description,
                    PickupDate = request.PickupDate,
                    DeliveryDate = request.DeliveryDate,
                    PickupAddress = request.PickupAddress,
                    PickupLatitude = request.PickupLatitude,
                    PickupLongitude = request.PickupLongitude,
                    DeliveryAddress = request.DeliveryAddress,
                    DeliveryLatitude = request.DeliveryLatitude,
                    DeliveryLongitude = request.DeliveryLongitude,
                    SpecialRequirements = request.SpecialRequirements,
                    GoodsType = request.GoodsType,
                    LoadTypeId = request.LoadTypeId,
                    Price = request.Price,
                    Currency = request.Currency,
                    Region = request.Region,
                    RequiredTruckTypeId = request.RequiredTruckTypeId,
                    IsStackable = request.IsStackable,
                    RequiresTemperatureControl = request.RequiresTemperatureControl,
                    HazardousMaterialClass = request.HazardousMaterialClass,
                    HandlingInstructions = request.HandlingInstructions,
                    IsFragile = request.IsFragile,
                    RequiresStackingControl = request.RequiresStackingControl,
                    StackingInstructions = request.StackingInstructions,
                    UNNumber = request.UNNumber,
                    RequiresCustomsDeclaration = request.RequiresCustomsDeclaration,
                    CustomsDeclarationNumber = request.CustomsDeclarationNumber
                };

                var createdLoad = await _shipperService.CreateLoadAsync(load, GetCurrentUserId());
                var response = MapToLoadResponse(createdLoad);
                
                return CreatedAtAction(nameof(GetLoadById), new { loadId = createdLoad.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating load");
                return StatusCode(500, "An error occurred while creating the load");
            }
        }

        [HttpGet("loads")]
        public async Task<ActionResult<IEnumerable<LoadResponse>>> GetLoads()
        {
            try
            {
                var loads = await _shipperService.GetShipperLoadsAsync(GetCurrentUserId());
                var responses = loads.Select(MapToLoadResponse);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving loads");
                return StatusCode(500, "An error occurred while retrieving loads");
            }
        }

        [HttpGet("loads/{loadId}")]
        public async Task<ActionResult<LoadResponse>> GetLoadById(long loadId)
        {
            try
            {
                var load = await _shipperService.GetShipperLoadByIdAsync(GetCurrentUserId(), loadId);
                if (load == null)
                {
                    return NotFound();
                }
                var response = MapToLoadResponse(load);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while retrieving the load");
            }
        }

        [HttpPut("loads/{loadId}")]
        public async Task<IActionResult> UpdateLoad(long loadId, [FromBody] LoadUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var load = await _shipperService.GetShipperLoadByIdAsync(GetCurrentUserId(), loadId);
                if (load == null)
                {
                    return NotFound();
                }

                // Update load properties
                load.Weight = request.Weight;
                load.Height = request.Height;
                load.Width = request.Width;
                load.Length = request.Length;
                load.Description = request.Description;
                load.PickupDate = request.PickupDate;
                load.DeliveryDate = request.DeliveryDate;
                load.SpecialRequirements = request.SpecialRequirements;
                load.GoodsType = request.GoodsType;
                load.LoadTypeId = request.LoadTypeId;
                load.Price = request.Price;
                load.Currency = request.Currency;
                load.Region = request.Region;
                load.RequiredTruckTypeId = request.RequiredTruckTypeId;
                load.IsStackable = request.IsStackable;
                load.RequiresTemperatureControl = request.RequiresTemperatureControl;
                load.HazardousMaterialClass = request.HazardousMaterialClass;
                load.HandlingInstructions = request.HandlingInstructions;
                load.IsFragile = request.IsFragile;
                load.RequiresStackingControl = request.RequiresStackingControl;
                load.StackingInstructions = request.StackingInstructions;
                load.UNNumber = request.UNNumber;
                load.RequiresCustomsDeclaration = request.RequiresCustomsDeclaration;
                load.CustomsDeclarationNumber = request.CustomsDeclarationNumber;

                var result = await _shipperService.UpdateLoadAsync(load, GetCurrentUserId());
                if (!result)
                {
                    return BadRequest("Failed to update load");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while updating the load");
            }
        }

        [HttpDelete("loads/{loadId}")]
        public async Task<IActionResult> DeleteLoad(long loadId)
        {
            try
            {
                var result = await _shipperService.DeleteLoadAsync(loadId, GetCurrentUserId());
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while deleting the load");
            }
        }

        #endregion

        #region Truck Search

        [HttpPost("search/trucks")]
        public async Task<ActionResult<IEnumerable<TruckSearchResponse>>> SearchTrucks([FromBody] SearchTrucksRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var trucks = await _shipperService.SearchAvailableTrucksAsync(
                    request.OriginLatitude,
                    request.OriginLongitude,
                    request.DestinationLatitude,
                    request.DestinationLongitude,
                    request.Weight,
                    request.Height,
                    request.Width,
                    request.Length,
                    request.PickupDate,
                    request.DeliveryDate);

                var responses = trucks.Select(MapToTruckSearchResponse);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for trucks");
                return StatusCode(500, "An error occurred while searching for trucks");
            }
        }

        #endregion

        #region Booking Management

        [HttpPost("bookings")]
        public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var booking = await _shipperService.CreateBookingRequestAsync(
                    request.LoadId,
                    request.TruckId,
                    GetCurrentUserId());

                var response = MapToBookingResponse(booking);
                return CreatedAtAction(nameof(GetBookingById), new { bookingId = booking.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, "An error occurred while creating the booking");
            }
        }

        [HttpGet("bookings")]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings()
        {
            try
            {
                var bookings = await _shipperService.GetShipperBookingsAsync(GetCurrentUserId());
                var responses = bookings.Select(MapToBookingResponse);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings");
                return StatusCode(500, "An error occurred while retrieving bookings");
            }
        }

        [HttpGet("bookings/{bookingId}")]
        public async Task<ActionResult<BookingResponse>> GetBookingById(long bookingId)
        {
            try
            {
                var booking = await _shipperService.GetShipperBookingByIdAsync(GetCurrentUserId(), bookingId);
                if (booking == null)
                {
                    return NotFound();
                }
                var response = MapToBookingResponse(booking);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking with ID {BookingId}", bookingId);
                return StatusCode(500, "An error occurred while retrieving the booking");
            }
        }

        [HttpPost("bookings/{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(long bookingId)
        {
            try
            {
                var result = await _shipperService.CancelBookingAsync(bookingId, GetCurrentUserId());
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking with ID {BookingId}", bookingId);
                return StatusCode(500, "An error occurred while cancelling the booking");
            }
        }

        #endregion

        #region Tracking

        [HttpGet("loads/{loadId}/location")]
        public async Task<ActionResult<TruckLocation>> GetLoadLocation(long loadId)
        {
            try
            {
                var location = await _shipperService.GetCurrentLoadLocationAsync(loadId, GetCurrentUserId());
                if (location == null)
                {
                    return NotFound();
                }
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving location for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while retrieving the load location");
            }
        }

        [HttpGet("loads/{loadId}/location/history")]
        public async Task<ActionResult<IEnumerable<object>>> GetLoadLocationHistory(long loadId)
        {
            try
            {
                var history = await _shipperService.GetLoadLocationHistoryAsync(loadId, GetCurrentUserId());
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving location history for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while retrieving the load location history");
            }
        }

        #endregion

        #region Mapping Methods

        private LoadResponse MapToLoadResponse(Load load)
        {
            return new LoadResponse
            {
                Id = load.Id,
                ShipperId = load.ShipperId ?? string.Empty,
                Weight = load.Weight,
                Height = load.Height,
                Width = load.Width,
                Length = load.Length,
                Description = load.Description,
                PickupDate = load.PickupDate,
                DeliveryDate = load.DeliveryDate,
                SpecialRequirements = load.SpecialRequirements,
                GoodsType = load.GoodsType,
                LoadTypeId = load.LoadTypeId,
                LoadTypeName = load.LoadType?.Name,
                Price = load.Price,
                Currency = load.Currency,
                Region = load.Region,
                RequiredTruckTypeId = load.RequiredTruckTypeId,
                RequiredTruckTypeName = load.RequiredTruckType?.Name,
                IsStackable = load.IsStackable,
                RequiresTemperatureControl = load.RequiresTemperatureControl,
                HazardousMaterialClass = load.HazardousMaterialClass,
                HandlingInstructions = load.HandlingInstructions,
                IsFragile = load.IsFragile,
                RequiresStackingControl = load.RequiresStackingControl,
                StackingInstructions = load.StackingInstructions,
                UNNumber = load.UNNumber,
                RequiresCustomsDeclaration = load.RequiresCustomsDeclaration,
                CustomsDeclarationNumber = load.CustomsDeclarationNumber,
                PickupAddress = load.PickupAddress,
                PickupLatitude = load.PickupLatitude,
                PickupLongitude = load.PickupLongitude,
                DeliveryAddress = load.DeliveryAddress,
                DeliveryLatitude = load.DeliveryLatitude,
                DeliveryLongitude = load.DeliveryLongitude,
                Status = load.Status,
                CreatedDate = load.CreatedDate,
                UpdatedDate = load.UpdatedDate,
                // Get current booking status if available
                CurrentBookingStatus = load.Status == LoadStatusEnum.Assigned ? 
                    BookingStatusEnum.Confirmed : null,
                CurrentBookingId = null // This will need to be populated from a different source
            };
        }

        private BookingResponse MapToBookingResponse(Booking booking)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                LoadId = booking.LoadId,
                TruckId = booking.TruckId,
                TruckRegistrationNumber = booking.Truck?.RegistrationNumber,
                DriverName = booking.Truck?.AssignedDriver?.FullName,
                DriverPhoneNumber = booking.Truck?.AssignedDriver?.User?.PhoneNumber,
                TruckCompanyName = booking.Truck?.CompanyName,
                AgreedPrice = booking.AgreedPrice,
                PricingModel = booking.PriceCalculationMethod,
                Currency = booking.Currency,
                Status = booking.Status,
                BookingDate = booking.CreatedDate,
                ConfirmationDate = booking.ConfirmationDate,
                CancellationDate = null, // Booking model doesn't have this property
                CancellationReason = booking.CancellationReason,
                PickupDate = booking.Load?.PickupDate,
                DeliveryDate = booking.Load?.DeliveryDate,
                ActualPickupDate = booking.PickupDate,
                ActualDeliveryDate = booking.DeliveryDateActual,
                Notes = null, // Booking model doesn't have this property
                IsExpressBooking = false, // Booking model doesn't have this property
                CreatedDate = booking.CreatedDate,
                UpdatedDate = booking.UpdatedDate,
                
                // Load summary information
                LoadWeight = booking.Load?.Weight ?? 0,
                LoadDescription = booking.Load?.Description,
                LoadGoodsType = booking.Load?.GoodsType ?? GoodsTypeEnum.General,
                
                // Current location information (if available)
                CurrentLatitude = null, // This would need to be populated from a location service
                CurrentLongitude = null,
                LastLocationUpdate = null,
                EstimatedTimeOfArrival = null
            };
        }

        private TruckSearchResponse MapToTruckSearchResponse(Truck truck)
        {
            return new TruckSearchResponse
            {
                Id = truck.Id,
                RegistrationNumber = truck.RegistrationNumber,
                TruckTypeId = truck.TruckTypeId,
                TruckTypeName = truck.TruckType?.Name ?? string.Empty,
                AvailableCapacityWeight = truck.AvailableCapacityWeight,
                Height = truck.Height ?? 0,
                Width = truck.Width ?? 0,
                Length = truck.Length ?? 0,
                VolumeCapacity = truck.VolumeCapacity,
                HasRefrigeration = truck.HasRefrigeration,
                HasLiftgate = truck.HasLiftgate,
                HasLoadingRamp = truck.HasLoadingRamp,
                CanTransportHazardousMaterials = truck.CanTransportHazardousMaterials,
                HazardousMaterialsClasses = truck.HazardousMaterialsClasses,
                AvailabilityStartDate = truck.AvailabilityStartDate,
                AvailabilityEndDate = truck.AvailabilityEndDate,
                OperationalStatus = truck.OperationalStatus,
                
                // Driver information
                DriverName = truck.AssignedDriver?.FullName,
                DriverPhoneNumber = truck.AssignedDriver?.User?.PhoneNumber,
                DriverRating = (double?)(truck.AssignedDriver?.SafetyRating),
                
                // Company information
                CompanyName = truck.CompanyName,
                CompanyPhoneNumber = truck.CompanyPhoneNumber,
                CompanyRating = truck.CompanyRating,
                
                // Location and route information (these would need to be populated from a location service)
                CurrentLatitude = null,
                CurrentLongitude = null,
                DistanceToPickup = (double?)(truck.DistanceToPickup),
                EstimatedTimeToPickup = null,
                RouteMatchPercentage = null,
                
                // Pricing information
                EstimatedPrice = null,
                Currency = "USD",
                PricingModel = null
            };
        }

        #endregion
    }
} 