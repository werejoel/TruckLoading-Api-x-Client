using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    [EnableRateLimiting("default")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserActivityController : ControllerBase
    {
        private readonly IUserActivityService _userActivityService;

        public UserActivityController(IUserActivityService userActivityService)
        {
            _userActivityService = userActivityService;
        }

        [HttpGet("user/{userId}")]
        [Authorize(Policy = "ViewUserActivities")]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetUserActivities(
            string userId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? activityType,
            [FromQuery] int? maxResults)
        {
            var activities = await _userActivityService.GetUserActivitiesAsync(
                userId,
                startDate,
                endDate,
                activityType,
                maxResults);

            return Ok(activities);
        }

        [HttpGet("entity/{entityType}/{entityId}")]
        [Authorize(Policy = "ViewUserActivities")]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetEntityActivities(
            string entityType,
            string entityId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var activities = await _userActivityService.GetActivitiesByEntityAsync(
                entityType,
                entityId,
                startDate,
                endDate);

            return Ok(activities);
        }

        [HttpGet("type/{activityType}")]
        [Authorize(Policy = "ViewUserActivities")]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetActivitiesByType(
            string activityType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? maxResults)
        {
            var activities = await _userActivityService.GetActivitiesByTypeAsync(
                activityType,
                startDate,
                endDate,
                maxResults);

            return Ok(activities);
        }

        [HttpGet("recent")]
        [Authorize(Policy = "ViewUserActivities")]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetRecentActivities(
            [FromQuery] string? userId,
            [FromQuery] int maxResults = 50)
        {
            var activities = await _userActivityService.GetRecentActivitiesAsync(userId, maxResults);
            return Ok(activities);
        }

        [HttpGet("statistics/{userId}")]
        [Authorize(Policy = "ViewUserActivities")]
        public async Task<ActionResult<Dictionary<string, int>>> GetActivityStatistics(
            string userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var statistics = await _userActivityService.GetActivityStatisticsAsync(
                userId,
                startDate,
                endDate);

            return Ok(statistics);
        }

        [HttpGet("search")]
        [Authorize(Policy = "ViewUserActivities")]
        public async Task<ActionResult<IEnumerable<UserActivity>>> SearchActivities(
            [FromQuery] string searchTerm,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? userId,
            [FromQuery] string? activityType,
            [FromQuery] int? maxResults)
        {
            var activities = await _userActivityService.SearchActivitiesAsync(
                searchTerm,
                startDate,
                endDate,
                userId,
                activityType,
                maxResults);

            return Ok(activities);
        }

        [HttpDelete("{activityId}")]
        [Authorize(Policy = "ManageUserActivities")]
        public async Task<IActionResult> DeleteActivity(long activityId)
        {
            var result = await _userActivityService.DeleteActivityAsync(activityId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("user/{userId}/older-than")]
        [Authorize(Policy = "ManageUserActivities")]
        public async Task<IActionResult> DeleteOldActivities(
            string userId,
            [FromQuery] DateTime olderThan)
        {
            var result = await _userActivityService.DeleteActivitiesAsync(userId, olderThan);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
