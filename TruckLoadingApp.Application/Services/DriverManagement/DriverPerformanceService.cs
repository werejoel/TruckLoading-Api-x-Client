using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;
using PerformanceMetrics = TruckLoadingApp.Application.Services.DriverManagement.Interfaces.PerformanceMetrics;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class DriverPerformanceService : IDriverPerformanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;

        public DriverPerformanceService(ApplicationDbContext context, IUserActivityService userActivityService)
        {
            _context = context;
            _userActivityService = userActivityService;
        }

        public async Task<DriverPerformance> RecordPerformanceAsync(DriverPerformance performance)
        {
            // Calculate overall performance score
            performance.OverallPerformanceScore = CalculateOverallScore(performance);

            _context.Add(performance);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                performance.Driver.UserId,
                ActivityTypes.RecordPerformance,
                $"Recorded performance for date {performance.Date}",
                "Performance",
                performance.Id.ToString());

            return performance;
        }

        public async Task<decimal> CalculateDriverRatingAsync(long driverId)
        {
            var recentPerformance = await _context.Set<DriverPerformance>()
                .Where(p => p.DriverId == driverId)
                .OrderByDescending(p => p.Date)
                .Take(10) // Consider last 10 performance records
                .ToListAsync();

            if (!recentPerformance.Any())
                return 0;

            return recentPerformance.Average(p => p.OverallPerformanceScore);
        }

        public async Task<PerformanceMetrics> GetDriverMetricsAsync(
            long driverId,
            DateTime startDate,
            DateTime endDate)
        {
            var performances = await _context.Set<DriverPerformance>()
                .Where(p => p.DriverId == driverId &&
                           p.Date >= startDate &&
                           p.Date <= endDate)
                .ToListAsync();

            if (!performances.Any())
                return new PerformanceMetrics();

            return new PerformanceMetrics
            {
                SafetyScore = performances.Average(p => p.SafetyScore),
                OnTimeDeliveryRate = performances.Average(p => p.OnTimeDeliveryRate),
                CustomerSatisfaction = performances.Average(p => p.CustomerRating),
                FuelEfficiency = performances.Average(p => p.FuelEfficiency),
                ComplianceViolations = performances.Sum(p => p.RestBreakViolations + p.HoursOfServiceViolations),
                TotalDrivingTime = TimeSpan.FromTicks((long)performances.Average(p => p.TotalDrivingTime.Ticks)),
                MaintenanceScore = (decimal)performances.Average(p => p.VehicleInspectionScore),
                OverallScore = performances.Average(p => p.OverallPerformanceScore)
            };
        }

        public async Task<DriverPerformance?> GetPerformanceByIdAsync(long performanceId)
        {
            return await _context.Set<DriverPerformance>()
                .Include(p => p.Driver)
                .FirstOrDefaultAsync(p => p.Id == performanceId);
        }

        public async Task<IEnumerable<DriverPerformance>> GetDriverPerformanceHistoryAsync(
            long driverId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.Set<DriverPerformance>()
                .Where(p => p.DriverId == driverId &&
                           p.Date >= startDate &&
                           p.Date <= endDate)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        private decimal CalculateOverallScore(DriverPerformance performance)
        {
            // Weighted scoring system
            const decimal safetyWeight = 0.35m;
            const decimal deliveryWeight = 0.25m;
            const decimal complianceWeight = 0.20m;
            const decimal maintenanceWeight = 0.20m;

            var safetyScore = performance.SafetyScore;
            var deliveryScore = performance.OnTimeDeliveryRate;
            var complianceScore = CalculateComplianceScore(performance);
            var maintenanceScore = performance.VehicleInspectionScore;

            return (safetyScore * safetyWeight) +
                   (deliveryScore * deliveryWeight) +
                   (complianceScore * complianceWeight) +
                   (maintenanceScore * maintenanceWeight);
        }

        private decimal CalculateComplianceScore(DriverPerformance performance)
        {
            const int maxViolations = 5;
            var totalViolations = performance.RestBreakViolations + performance.HoursOfServiceViolations;
            var violationScore = Math.Max(0, maxViolations - totalViolations) * (100.0m / maxViolations);
            return violationScore;
        }

        public decimal CalculateSafetyScore(List<DriverPerformance> performances)
        {
            if (!performances.Any()) return 0;

            // Calculate average safety score based on the SafetyScore property of each DriverPerformance object.
            return performances.Average(p => p.SafetyScore);
        }
    }
}
