using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IDriverService _driverService;

        public DriverController(UserManager<User> userManager, IDriverService driverService)
        {
            _userManager = userManager;
            _driverService = driverService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Company,Trucker")]
        public async Task<IActionResult> RegisterDriver([FromBody] DriverRegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not found");

            var driver = new Driver
            {
                LicenseNumber = request.LicenseNumber,
                LicenseExpiryDate = request.LicenseExpiryDate,
                Experience = request.Experience,
                SafetyRating = request.SafetyRating
            };

            var result = await _driverService.RegisterDriver(driver, userId);
            return result ? Ok("Driver registered successfully") : BadRequest("Failed to register driver");
        }

        [HttpPost("assign/{driverId}/{truckId}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> AssignDriverToTruck(long driverId, int truckId)
        {
            var result = await _driverService.AssignDriverToTruck((int)driverId, truckId);
            return result ? Ok("Driver assigned successfully") : BadRequest("Failed to assign driver");
        }

        [HttpPost("unassign/{driverId}")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UnassignDriver(int driverId)
        {
            var result = await _driverService.UnassignDriver(driverId);
            return result ? Ok("Driver unassigned successfully") : BadRequest("Failed to unassign driver");
        }
    }
}
