using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TruckController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITruckService _truckService;

        public TruckController(UserManager<User> userManager, ITruckService truckService)
        {
            _userManager = userManager;
            _truckService = truckService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Company,Trucker")]
        public async Task<IActionResult> RegisterTruck([FromBody] TruckRegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not found");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized("User not found");

            var isIndividualTrucker = user.UserType == UserType.Trucker;

            // 🚀 Ensure truckers cannot register more than one truck
            if (isIndividualTrucker)
            {
                var existingTruck = await _truckService.GetTruckByOwnerId(userId);
                if (existingTruck != null)
                {
                    return BadRequest("Truckers can only register one truck. Register as a company for multiple trucks.");
                }
            }

            var truck = new Truck
            {
                TruckTypeId = request.TruckTypeId,
                NumberPlate = request.NumberPlate,
                LoadCapacityWeight = request.LoadCapacityWeight,
                LoadCapacityVolume = request.LoadCapacityVolume,
                Height = request.Height,
                Width = request.Width,
                Length = request.Length,
                AvailabilityStartDate = request.AvailabilityStartDate,
                AvailabilityEndDate = request.AvailabilityEndDate
            };

            var result = await _truckService.RegisterTruck(truck, userId, isIndividualTrucker);
            return result ? Ok("Truck registered successfully, pending approval") : BadRequest("Failed to register truck");
        }

        [HttpGet("unapproved")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUnapprovedTrucks()
        {
            var trucks = await _truckService.GetUnapprovedTrucks();
            return Ok(trucks);
        }

        [HttpPost("approve/{truckId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveTruck(int truckId)
        {
            var result = await _truckService.ApproveTruck(truckId);
            return result ? Ok("Truck approved successfully") : BadRequest("Failed to approve truck");
        }
    }
}