using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly ApplicationDbContext _context;

        public UserActivityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(
            string userId,
            string activityType,
            string description,
            string? entityType = null,
            string? entityId = null,
            string? status = null,
            string? metadata = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                ActivityType = activityType,
                Description = description,
                EntityType = entityType,
                EntityId = entityId,
                Status = status,
                Metadata = metadata,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _context.UserActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(
            string userId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? activityType = null,
            int? maxResults = null)
        {
            var query = _context.UserActivities
                .Where(ua => ua.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(ua => ua.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(ua => ua.Timestamp <= endDate.Value);

            if (!string.IsNullOrEmpty(activityType))
                query = query.Where(ua => ua.ActivityType == activityType);

            query = query.OrderByDescending(ua => ua.Timestamp);

            if (maxResults.HasValue)
                query = query.Take(maxResults.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetActivitiesByEntityAsync(
            string entityType,
            string entityId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.UserActivities
                .Where(ua => ua.EntityType == entityType && ua.EntityId == entityId);

            if (startDate.HasValue)
                query = query.Where(ua => ua.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(ua => ua.Timestamp <= endDate.Value);

            return await query
                .OrderByDescending(ua => ua.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetActivitiesByTypeAsync(
            string activityType,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null)
        {
            var query = _context.UserActivities
                .Where(ua => ua.ActivityType == activityType);

            if (startDate.HasValue)
                query = query.Where(ua => ua.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(ua => ua.Timestamp <= endDate.Value);

            query = query.OrderByDescending(ua => ua.Timestamp);

            if (maxResults.HasValue)
                query = query.Take(maxResults.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(
            string? userId = null,
            int maxResults = 50)
        {
            var query = _context.UserActivities.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(ua => ua.UserId == userId);

            return await query
                .OrderByDescending(ua => ua.Timestamp)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetActivityStatisticsAsync(
            string userId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.UserActivities
                .Where(ua => ua.UserId == userId &&
                           ua.Timestamp >= startDate &&
                           ua.Timestamp <= endDate)
                .GroupBy(ua => ua.ActivityType)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Count());
        }

        public async Task<IEnumerable<UserActivity>> SearchActivitiesAsync(
            string searchTerm,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? userId = null,
            string? activityType = null,
            int? maxResults = null)
        {
            var query = _context.UserActivities.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(ua => ua.UserId == userId);

            if (!string.IsNullOrEmpty(activityType))
                query = query.Where(ua => ua.ActivityType == activityType);

            if (startDate.HasValue)
                query = query.Where(ua => ua.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(ua => ua.Timestamp <= endDate.Value);

            query = query.Where(ua =>
                ua.Description.Contains(searchTerm) ||
                (ua.EntityType != null && ua.EntityType.Contains(searchTerm)) ||
                (ua.EntityId != null && ua.EntityId.Contains(searchTerm)) ||
                (ua.Status != null && ua.Status.Contains(searchTerm)));

            query = query.OrderByDescending(ua => ua.Timestamp);

            if (maxResults.HasValue)
                query = query.Take(maxResults.Value);

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteActivityAsync(long activityId)
        {
            var activity = await _context.UserActivities.FindAsync(activityId);
            if (activity == null)
                return false;

            _context.UserActivities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteActivitiesAsync(
            string userId,
            DateTime olderThan)
        {
            var activities = await _context.UserActivities
                .Where(ua => ua.UserId == userId && ua.Timestamp < olderThan)
                .ToListAsync();

            if (!activities.Any())
                return false;

            _context.UserActivities.RemoveRange(activities);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
