using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    /// <summary>
    /// Service that performs compliance validation without dependencies on other driver services,
    /// breaking the circular dependency between DriverScheduleService and DriverComplianceService
    /// </summary>
    public class ComplianceCheckerService
    {
        private readonly ApplicationDbContext _context;

        public ComplianceCheckerService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Validates if a driver has had proper rest periods before a new schedule
        /// </summary>
        public async Task<bool> ValidateRestComplianceAsync(long driverId, DateTime scheduleStart, DateTime scheduleEnd)
        {
            // Get the last rest period before this schedule
            var lastRestPeriod = await _context.Set<DriverRestPeriod>()
                .Where(r => r.DriverId == driverId && 
                          r.EndTime <= scheduleStart &&
                          r.Status == RestStatus.Completed)
                .OrderByDescending(r => r.EndTime)
                .FirstOrDefaultAsync();

            if (lastRestPeriod == null)
            {
                // If no rest period found, check if this is the driver's first schedule
                var hasExistingSchedules = await _context.Set<DriverSchedule>()
                    .AnyAsync(s => s.DriverId == driverId);
                
                // Allow the schedule if this is the first one
                return !hasExistingSchedules;
            }

            // Check if enough time has passed since last daily rest
            if (lastRestPeriod.Type == RestType.DailyRest || lastRestPeriod.Type == RestType.WeeklyRest)
            {
                // For daily/weekly rest, check if driver has been resting for the minimum required time
                var restDuration = scheduleStart - lastRestPeriod.EndTime;
                return restDuration.TotalHours >= RestRegulations.MinimumRestHours;
            }

            // If it's just a short break, check if the driver hasn't exceeded maximum driving period
            var lastDrivingSchedule = await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId && 
                          s.EndTime <= lastRestPeriod.StartTime)
                .OrderByDescending(s => s.EndTime)
                .FirstOrDefaultAsync();

            if (lastDrivingSchedule == null)
                return true;

            // Calculate total continuous driving time
            var totalDrivingTime = (lastRestPeriod.StartTime - lastDrivingSchedule.StartTime).TotalHours;
            return totalDrivingTime <= RestRegulations.MaximumDrivingPeriod;
        }

        /// <summary>
        /// Validates that a schedule doesn't exceed maximum continuous driving time without breaks
        /// </summary>
        public async Task<bool> ValidateContinuousDrivingAsync(
            long driverId, 
            DateTime scheduleStart, 
            DateTime scheduleEnd,
            long? excludeScheduleId = null)
        {
            // Get previous schedule that ends before this one starts
            var previousSchedule = await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId && 
                          s.EndTime <= scheduleStart &&
                          s.Status != ScheduleStatus.Cancelled &&
                          (excludeScheduleId == null || s.Id != excludeScheduleId))
                .OrderByDescending(s => s.EndTime)
                .FirstOrDefaultAsync();

            if (previousSchedule == null)
                return true;

            // Check if there was a rest period between the previous schedule and this one
            var hasBrokenDrivingPeriod = await _context.Set<DriverRestPeriod>()
                .AnyAsync(r => r.DriverId == driverId && 
                             r.StartTime >= previousSchedule.EndTime &&
                             r.EndTime <= scheduleStart &&
                             r.Status == RestStatus.Completed &&
                             (r.EndTime - r.StartTime).TotalMinutes >= RestRegulations.MinimumBreakDuration);

            if (hasBrokenDrivingPeriod)
                return true;

            // Calculate continuous driving time without a break
            var timeBetweenSchedules = (scheduleStart - previousSchedule.EndTime).TotalHours;
            var continuousDrivingTime = (scheduleEnd - previousSchedule.StartTime).TotalHours - timeBetweenSchedules;

            return continuousDrivingTime <= RestRegulations.RequiredBreakAfter;
        }

        /// <summary>
        /// Calculates total driving time for a driver within a specific day, including a new schedule
        /// </summary>
        public async Task<TimeSpan> CalculateDailyDrivingTimeAsync(
            long driverId, 
            DateTime dayStart, 
            DateTime dayEnd, 
            DriverSchedule newSchedule,
            long? excludeScheduleId = null)
        {
            var existingSchedules = await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId &&
                          s.Status != ScheduleStatus.Cancelled &&
                          s.StartTime < dayEnd &&
                          s.EndTime > dayStart &&
                          (excludeScheduleId == null || s.Id != excludeScheduleId))
                .ToListAsync();

            // Add the new schedule to the calculation
            var allSchedules = new List<DriverSchedule>(existingSchedules);
            allSchedules.Add(newSchedule);

            // Calculate total driving time
            TimeSpan totalDrivingTime = TimeSpan.Zero;
            foreach (var schedule in allSchedules)
            {
                // Adjust start and end times to be within the day
                var effectiveStart = schedule.StartTime < dayStart ? dayStart : schedule.StartTime;
                var effectiveEnd = schedule.EndTime > dayEnd ? dayEnd : schedule.EndTime;
                
                if (effectiveStart < effectiveEnd)
                {
                    totalDrivingTime += effectiveEnd - effectiveStart;
                }
            }

            return totalDrivingTime;
        }

        /// <summary>
        /// Check if a driver is available for a specific time slot
        /// </summary>
        public async Task<bool> CheckDriverAvailabilityAsync(long driverId, DateTime startTime, DateTime endTime)
        {
            // Check if driver exists and is available
            var driver = await _context.Set<Driver>()
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null || !driver.IsAvailable)
                return false;

            // Check for scheduling conflicts
            var hasConflict = await _context.Set<DriverSchedule>()
                .AnyAsync(s => s.DriverId == driverId &&
                            s.Status != ScheduleStatus.Cancelled &&
                            ((s.StartTime <= startTime && s.EndTime > startTime) ||
                            (s.StartTime < endTime && s.EndTime >= endTime)));

            if (hasConflict)
                return false;

            // Check daily driving time limit
            var scheduleDayStart = startTime.Date;
            var scheduleDayEnd = scheduleDayStart.AddDays(1);
            
            var tentativeSchedule = new DriverSchedule
            {
                DriverId = driverId,
                StartTime = startTime,
                EndTime = endTime
            };
            
            var dailyDrivingTime = await CalculateDailyDrivingTimeAsync(
                driverId, 
                scheduleDayStart, 
                scheduleDayEnd, 
                tentativeSchedule);

            if (dailyDrivingTime > TimeSpan.FromHours(RestRegulations.MaxDrivingHoursPerDay))
                return false;

            // Check rest compliance
            var isRestCompliant = await ValidateRestComplianceAsync(driverId, startTime, endTime);
            if (!isRestCompliant)
                return false;

            // Check continuous driving time
            var isContinuousDrivingCompliant = await ValidateContinuousDrivingAsync(driverId, startTime, endTime);
            return isContinuousDrivingCompliant;
        }
    }
} 