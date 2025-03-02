using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class DriverScheduleService : IDriverScheduleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;
        private readonly ComplianceCheckerService _complianceChecker;

        public DriverScheduleService(
            ApplicationDbContext context, 
            IUserActivityService userActivityService,
            ComplianceCheckerService complianceChecker)
        {
            _context = context;
            _userActivityService = userActivityService;
            _complianceChecker = complianceChecker;
        }

        public async Task<DriverSchedule> CreateScheduleAsync(DriverSchedule schedule)
        {
            // Validate schedule times
            if (schedule.StartTime >= schedule.EndTime)
                throw new ArgumentException("End time must be after start time");

            // Check for overlapping schedules
            var hasOverlap = await _context.Set<DriverSchedule>()
                .AnyAsync(s => s.DriverId == schedule.DriverId &&
                              s.Status != ScheduleStatus.Cancelled &&
                              ((s.StartTime <= schedule.StartTime && s.EndTime > schedule.StartTime) ||
                               (s.StartTime < schedule.EndTime && s.EndTime >= schedule.EndTime)));

            if (hasOverlap)
                throw new InvalidOperationException("Schedule overlaps with existing schedule");

            // Calculate total driving time for the day
            var scheduleDayStart = schedule.StartTime.Date;
            var scheduleDayEnd = scheduleDayStart.AddDays(1);
            
            var dailyDrivingTime = await _complianceChecker.CalculateDailyDrivingTimeAsync(
                schedule.DriverId, 
                scheduleDayStart, 
                scheduleDayEnd, 
                schedule);

            // Check if this would exceed daily driving limit
            if (dailyDrivingTime > TimeSpan.FromHours(RestRegulations.MaxDrivingHoursPerDay))
                throw new InvalidOperationException($"Schedule would exceed maximum daily driving time of {RestRegulations.MaxDrivingHoursPerDay} hours");

            // Check if driver has had sufficient rest since last driving period
            var isRestCompliant = await _complianceChecker.ValidateRestComplianceAsync(
                schedule.DriverId, schedule.StartTime, schedule.EndTime);
                
            if (!isRestCompliant)
                throw new InvalidOperationException("Schedule violates required rest period regulations");

            // Check continuous driving time without breaks
            var isContinuousDrivingCompliant = await _complianceChecker.ValidateContinuousDrivingAsync(
                schedule.DriverId, schedule.StartTime, schedule.EndTime);
                
            if (!isContinuousDrivingCompliant)
                throw new InvalidOperationException($"Schedule exceeds maximum continuous driving period of {RestRegulations.RequiredBreakAfter} hours without a break");

            _context.Add(schedule);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                schedule.Driver.UserId,
                ActivityTypes.CreateSchedule,
                $"Created schedule from {schedule.StartTime} to {schedule.EndTime}",
                "Schedule",
                schedule.Id.ToString());

            return schedule;
        }

        public async Task<DriverSchedule?> GetScheduleByIdAsync(long scheduleId)
        {
            return await _context.Set<DriverSchedule>()
                .Include(s => s.Driver)
                .Include(s => s.Load)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);
        }

        public async Task<IEnumerable<DriverSchedule>> GetDriverSchedulesAsync(long driverId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId &&
                           s.StartTime >= startDate &&
                           s.EndTime <= endDate)
                .Include(s => s.Load)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> UpdateScheduleAsync(DriverSchedule schedule)
        {
            var existing = await _context.Set<DriverSchedule>()
                .FindAsync(schedule.Id);

            if (existing == null)
                return false;

            // Check for overlapping schedules if times are being updated
            if (existing.StartTime != schedule.StartTime || existing.EndTime != schedule.EndTime)
            {
                var hasOverlap = await _context.Set<DriverSchedule>()
                    .AnyAsync(s => s.Id != schedule.Id &&
                                  s.DriverId == schedule.DriverId &&
                                  s.Status != ScheduleStatus.Cancelled &&
                                  ((s.StartTime <= schedule.StartTime && s.EndTime > schedule.StartTime) ||
                                   (s.StartTime < schedule.EndTime && s.EndTime >= schedule.EndTime)));

                if (hasOverlap)
                    throw new InvalidOperationException("Schedule overlaps with existing schedule");
                
                // Calculate total driving time for the day with the updated schedule
                var scheduleDayStart = schedule.StartTime.Date;
                var scheduleDayEnd = scheduleDayStart.AddDays(1);
                
                var dailyDrivingTime = await _complianceChecker.CalculateDailyDrivingTimeAsync(
                    schedule.DriverId, 
                    scheduleDayStart, 
                    scheduleDayEnd, 
                    schedule,
                    existing.Id);

                // Check if this would exceed daily driving limit
                if (dailyDrivingTime > TimeSpan.FromHours(RestRegulations.MaxDrivingHoursPerDay))
                    throw new InvalidOperationException($"Schedule would exceed maximum daily driving time of {RestRegulations.MaxDrivingHoursPerDay} hours");

                // Check if driver has had sufficient rest since last driving period
                var isRestCompliant = await _complianceChecker.ValidateRestComplianceAsync(
                    schedule.DriverId, schedule.StartTime, schedule.EndTime);
                    
                if (!isRestCompliant)
                    throw new InvalidOperationException("Schedule violates required rest period regulations");

                // Check continuous driving time without breaks
                var isContinuousDrivingCompliant = await _complianceChecker.ValidateContinuousDrivingAsync(
                    schedule.DriverId, 
                    schedule.StartTime, 
                    schedule.EndTime,
                    existing.Id);
                    
                if (!isContinuousDrivingCompliant)
                    throw new InvalidOperationException($"Schedule exceeds maximum continuous driving period of {RestRegulations.RequiredBreakAfter} hours without a break");
            }

            _context.Entry(existing).CurrentValues.SetValues(schedule);
            existing.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                existing.Driver.UserId,
                ActivityTypes.UpdateSchedule,
                $"Updated schedule from {schedule.StartTime} to {schedule.EndTime}",
                "Schedule",
                existing.Id.ToString());

            return true;
        }

        public async Task<bool> DeleteScheduleAsync(long scheduleId)
        {
            var schedule = await _context.Set<DriverSchedule>()
                .Include(s => s.Driver)
                .Include(s => s.Load)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
                return false;

            if (schedule.LoadId.HasValue)
            {
                // Don't allow deletion if there's an associated load that hasn't been completed or canceled
                if (schedule.Load != null && 
                   schedule.Load.Status != LoadStatusEnum.Delivered && 
                   schedule.Load.Status != LoadStatusEnum.Cancelled)
                {
                    throw new InvalidOperationException("Cannot delete schedule with an active load");
                }
            }

            // Mark as cancelled instead of deleting
            schedule.Status = ScheduleStatus.Cancelled;
            schedule.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                schedule.Driver.UserId,
                ActivityTypes.DeleteSchedule,
                $"Cancelled schedule from {schedule.StartTime} to {schedule.EndTime}",
                "Schedule",
                scheduleId.ToString());

            return true;
        }

        public async Task<IEnumerable<DriverSchedule>> GetAvailableDriversForTimeSlotAsync(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("End time must be after start time");

            // Get all available drivers
            var availableDrivers = await _context.Set<Driver>()
                .Include(d => d.User)
                .Where(d => d.IsAvailable)
                .ToListAsync();

            var result = new List<DriverSchedule>();

            foreach (var driver in availableDrivers)
            {
                // Check for scheduling conflicts
                var hasConflict = await _context.Set<DriverSchedule>()
                    .AnyAsync(s => s.DriverId == driver.Id &&
                                  s.Status != ScheduleStatus.Cancelled &&
                                  ((s.StartTime <= startTime && s.EndTime > startTime) ||
                                   (s.StartTime < endTime && s.EndTime >= endTime)));

                if (hasConflict)
                    continue;

                // Calculate daily driving time for this time slot
                var scheduleDayStart = startTime.Date;
                var scheduleDayEnd = scheduleDayStart.AddDays(1);
                
                var tentativeSchedule = new DriverSchedule
                {
                    DriverId = driver.Id,
                    StartTime = startTime,
                    EndTime = endTime
                };
                
                var dailyDrivingTime = await _complianceChecker.CalculateDailyDrivingTimeAsync(
                    driver.Id, 
                    scheduleDayStart, 
                    scheduleDayEnd, 
                    tentativeSchedule);

                // Skip driver if this would exceed daily driving limit
                if (dailyDrivingTime > TimeSpan.FromHours(RestRegulations.MaxDrivingHoursPerDay))
                    continue;

                // Skip driver if rest period compliance would be violated
                var isRestCompliant = await _complianceChecker.ValidateRestComplianceAsync(
                    driver.Id, startTime, endTime);
                    
                if (!isRestCompliant)
                    continue;

                // Skip driver if continuous driving without breaks would be violated
                var isContinuousDrivingCompliant = await _complianceChecker.ValidateContinuousDrivingAsync(
                    driver.Id, startTime, endTime);
                    
                if (!isContinuousDrivingCompliant)
                    continue;

                // Driver is available for this slot
                result.Add(new DriverSchedule
                {
                    DriverId = driver.Id,
                    Driver = driver,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = ScheduleStatus.Pending
                });
            }

            return result;
        }

        public async Task<bool> IsDriverAvailableAsync(long driverId, DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("End time must be after start time");

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
            
            var dailyDrivingTime = await _complianceChecker.CalculateDailyDrivingTimeAsync(
                driverId, 
                scheduleDayStart, 
                scheduleDayEnd, 
                tentativeSchedule);

            if (dailyDrivingTime > TimeSpan.FromHours(RestRegulations.MaxDrivingHoursPerDay))
                return false;

            // Check rest compliance
            var isRestCompliant = await _complianceChecker.ValidateRestComplianceAsync(
                driverId, startTime, endTime);
                
            if (!isRestCompliant)
                return false;

            // Check continuous driving time
            var isContinuousDrivingCompliant = await _complianceChecker.ValidateContinuousDrivingAsync(
                driverId, startTime, endTime);
                
            return isContinuousDrivingCompliant;
        }

        // Recurring schedule methods
        public async Task<IEnumerable<DriverSchedule>> CreateRecurringScheduleAsync(DriverSchedule schedule, RecurrencePattern pattern, DateTime endDate, int? maxOccurrences = null)
        {
            // Validate schedule times
            if (schedule.StartTime >= schedule.EndTime)
                throw new ArgumentException("End time must be after start time");

            // Configure schedule as recurring
            schedule.IsRecurring = true;
            schedule.RecurrencePattern = pattern;
            schedule.RecurrenceEndDate = endDate;

            // Save the parent schedule
            _context.Add(schedule);
            await _context.SaveChangesAsync();

            var instances = new List<DriverSchedule>();
            instances.Add(schedule); // Include the parent schedule

            // Generate recurring instances
            var currentStart = schedule.StartTime;
            var currentEnd = schedule.EndTime;
            var instanceNumber = 1;
            
            // Calculate the duration of each schedule
            TimeSpan duration = currentEnd - currentStart;
            
            while ((currentStart <= endDate) && (!maxOccurrences.HasValue || instances.Count < maxOccurrences.Value))
            {
                // Calculate next occurrence based on pattern
                switch (pattern)
                {
                    case RecurrencePattern.Daily:
                        currentStart = currentStart.AddDays(1);
                        break;
                    case RecurrencePattern.Weekly:
                        currentStart = currentStart.AddDays(7);
                        break;
                    case RecurrencePattern.BiWeekly:
                        currentStart = currentStart.AddDays(14);
                        break;
                    case RecurrencePattern.Monthly:
                        currentStart = currentStart.AddMonths(1);
                        break;
                }
                
                if (currentStart > endDate)
                    break;
                
                // Create new instance
                var instance = new DriverSchedule
                {
                    DriverId = schedule.DriverId,
                    StartTime = currentStart,
                    EndTime = currentStart.Add(duration),
                    Status = ScheduleStatus.Pending,
                    Notes = schedule.Notes,
                    IsRecurring = false, // Instances are not recurring themselves
                    RecurringScheduleId = schedule.Id,
                    InstanceNumber = instanceNumber
                };
                
                // Check compliance for each instance
                var isCompliant = await _complianceChecker.ValidateRestComplianceAsync(instance.DriverId, instance.StartTime, instance.EndTime);
                var isContinuousDrivingCompliant = await _complianceChecker.ValidateContinuousDrivingAsync(instance.DriverId, instance.StartTime, instance.EndTime);
                
                if (isCompliant && isContinuousDrivingCompliant)
                {
                    _context.Add(instance);
                    instances.Add(instance);
                }
                
                instanceNumber++;
            }
            
            await _context.SaveChangesAsync();
            
            await _userActivityService.LogActivityAsync(
                schedule.Driver.UserId,
                ActivityTypes.CreateSchedule,
                $"Created recurring {pattern} schedule from {schedule.StartTime} to {endDate}",
                "RecurringSchedule",
                schedule.Id.ToString());
            
            return instances;
        }

        public async Task<IEnumerable<DriverSchedule>> GetRecurringScheduleInstancesAsync(long recurringScheduleId)
        {
            return await _context.Set<DriverSchedule>()
                .Where(s => s.RecurringScheduleId == recurringScheduleId || s.Id == recurringScheduleId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> UpdateRecurringScheduleAsync(DriverSchedule schedule, bool applyToAllInstances)
        {
            var existingSchedule = await _context.Set<DriverSchedule>()
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == schedule.Id);
                
            if (existingSchedule == null || !existingSchedule.IsRecurring)
                return false;
                
            // Update the parent schedule
            _context.Entry(existingSchedule).CurrentValues.SetValues(schedule);
            existingSchedule.UpdatedDate = DateTime.UtcNow;
            
            if (applyToAllInstances)
            {
                // Get all instances
                var instances = await _context.Set<DriverSchedule>()
                    .Where(s => s.RecurringScheduleId == schedule.Id)
                    .ToListAsync();
                    
                foreach (var instance in instances)
                {
                    // Keep relative timing but update other properties
                    var timeDifference = instance.StartTime - existingSchedule.StartTime;
                    var duration = schedule.EndTime - schedule.StartTime;
                    
                    instance.StartTime = schedule.StartTime.Add(timeDifference);
                    instance.EndTime = instance.StartTime.Add(duration);
                    instance.Notes = schedule.Notes;
                    instance.UpdatedDate = DateTime.UtcNow;
                    
                    // Validate compliance for each updated instance
                    var isCompliant = await _complianceChecker.ValidateRestComplianceAsync(instance.DriverId, instance.StartTime, instance.EndTime);
                    var isContinuousDrivingCompliant = await _complianceChecker.ValidateContinuousDrivingAsync(instance.DriverId, instance.StartTime, instance.EndTime);
                    
                    if (!isCompliant || !isContinuousDrivingCompliant)
                    {
                        // Mark non-compliant instances as requiring review
                        instance.Status = ScheduleStatus.Pending;
                        instance.Notes = (instance.Notes ?? "") + " [WARNING: Compliance check needed]";
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            
            await _userActivityService.LogActivityAsync(
                existingSchedule.Driver.UserId,
                ActivityTypes.UpdateSchedule,
                $"Updated recurring schedule {(applyToAllInstances ? "and all instances" : "")}",
                "RecurringSchedule",
                existingSchedule.Id.ToString());
                
            return true;
        }

        public async Task<bool> DeleteRecurringScheduleAsync(long scheduleId, bool deleteAllInstances)
        {
            var schedule = await _context.Set<DriverSchedule>()
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.IsRecurring);
                
            if (schedule == null)
                return false;
                
            if (deleteAllInstances)
            {
                // Find all future instances of this recurring schedule
                var instances = await _context.Set<DriverSchedule>()
                    .Where(s => s.RecurringScheduleId == scheduleId && s.StartTime > DateTime.UtcNow)
                    .ToListAsync();
                    
                foreach (var instance in instances)
                {
                    // Don't allow deletion if there's an associated load
                    if (instance.LoadId.HasValue)
                    {
                        // Check if the load is active
                        var load = await _context.Set<Load>()
                            .FindAsync(instance.LoadId.Value);
                        
                        if (load != null && 
                           load.Status != LoadStatusEnum.Delivered && 
                           load.Status != LoadStatusEnum.Cancelled)
                        {
                            // Mark as cancelled instead of deleting
                            instance.Status = ScheduleStatus.Cancelled;
                            instance.UpdatedDate = DateTime.UtcNow;
                        }
                        else
                        {
                            _context.Remove(instance);
                        }
                    }
                    else
                    {
                        _context.Remove(instance);
                    }
                }
            }
            
            // Mark parent as cancelled or delete if no instances exist
            var hasInstances = await _context.Set<DriverSchedule>()
                .AnyAsync(s => s.RecurringScheduleId == scheduleId);
                
            if (hasInstances && !deleteAllInstances)
            {
                schedule.Status = ScheduleStatus.Cancelled;
                schedule.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                _context.Remove(schedule);
            }
            
            await _context.SaveChangesAsync();
            
            await _userActivityService.LogActivityAsync(
                schedule.Driver.UserId,
                ActivityTypes.DeleteSchedule,
                $"Deleted recurring schedule {(deleteAllInstances ? "and all instances" : "")}",
                "RecurringSchedule",
                scheduleId.ToString());
                
            return true;
        }

        public async Task<IEnumerable<DriverSchedule>> GetDriverRecurringSchedulesAsync(long driverId)
        {
            return await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId && s.IsRecurring && s.Status != ScheduleStatus.Cancelled)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
