using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IUserActivityService
    {
        Task LogActivityAsync(
            string userId,
            string activityType,
            string description,
            string? entityType = null,
            string? entityId = null,
            string? status = null,
            string? metadata = null,
            string? ipAddress = null,
            string? userAgent = null);

        Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(
            string userId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? activityType = null,
            int? maxResults = null);

        Task<IEnumerable<UserActivity>> GetActivitiesByEntityAsync(
            string entityType,
            string entityId,
            DateTime? startDate = null,
            DateTime? endDate = null);

        Task<IEnumerable<UserActivity>> GetActivitiesByTypeAsync(
            string activityType,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null);

        Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(
            string? userId = null,
            int maxResults = 50);

        Task<Dictionary<string, int>> GetActivityStatisticsAsync(
            string userId,
            DateTime startDate,
            DateTime endDate);

        Task<IEnumerable<UserActivity>> SearchActivitiesAsync(
            string searchTerm,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? userId = null,
            string? activityType = null,
            int? maxResults = null);

        Task<bool> DeleteActivityAsync(long activityId);

        Task<bool> DeleteActivitiesAsync(
            string userId,
            DateTime olderThan);
    }
}
