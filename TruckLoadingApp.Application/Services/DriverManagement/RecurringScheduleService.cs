using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class RecurringScheduleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDriverScheduleService _driverScheduleService;
        private readonly IDriverComplianceService _driverComplianceService;
        private readonly IUserActivityService _userActivityService;

        public RecurringScheduleService(
            ApplicationDbContext context,
            IDriverScheduleService driverScheduleService,
            IDriverComplianceService driverComplianceService,
            IUserActivityService userActivityService)
        {
            _context = context;
            _driverScheduleService = driverScheduleService;
            _driverComplianceService = driverComplianceService;
            _userActivityService = userActivityService;
        }

        /// <summary>
        /// Creates a parent recurring schedule and generates the first batch of instances
        /// </summary>
        public async Task<DriverSchedule> CreateRecurringScheduleAsync(
            DriverSchedule parentSchedule, 
            RecurrencePattern pattern, 
            DateTime endDate, 
            int? maxOccurrences = null)
        {
            // Configure the parent schedule as recurring
            parentSchedule.IsRecurring = true;
            parentSchedule.RecurrencePattern = pattern;
            parentSchedule.RecurrenceEndDate = endDate;

            // Save the parent schedule
            _context.Add(parentSchedule);
            await _context.SaveChangesAsync();

            // Generate recurring instances
            var instances = GenerateScheduleInstances(parentSchedule, pattern, endDate, maxOccurrences);
            
            // Save recurring instances
            foreach (var instance in instances)
            {
                // Check for rest compliance for each instance
                var isCompliant = await _driverScheduleService.IsDriverAvailableAsync(
                    instance.DriverId, 
                    instance.StartTime, 
                    instance.EndTime);
                
                if (isCompliant)
                {
                    var scheduleInstance = new RecurringScheduleInstance
                    {
                        ParentScheduleId = parentSchedule.Id,
                        DriverId = instance.DriverId,
                        StartTime = instance.StartTime,
                        EndTime = instance.EndTime,
                        Status = ScheduleStatus.Pending,
                        Notes = parentSchedule.Notes,
                        InstanceNumber = instance.InstanceNumber ?? 0
                    };
                    
                    _context.Add(scheduleInstance);
                }
            }
            
            await _context.SaveChangesAsync();
            
            await _userActivityService.LogActivityAsync(
                parentSchedule.Driver.UserId,
                ActivityTypes.CreateSchedule,
                $"Created recurring {pattern} schedule from {parentSchedule.StartTime} to {endDate}",
                "RecurringSchedule",
                parentSchedule.Id.ToString());
            
            return parentSchedule;
        }

        /// <summary>
        /// Generates schedule instances based on recurrence pattern
        /// </summary>
        private List<DriverSchedule> GenerateScheduleInstances(
            DriverSchedule parentSchedule, 
            RecurrencePattern pattern, 
            DateTime endDate,
            int? maxOccurrences = null)
        {
            var instances = new List<DriverSchedule>();
            var currentStart = parentSchedule.StartTime;
            var currentEnd = parentSchedule.EndTime;
            var instanceNumber = 1;
            
            // Calculate the duration of each schedule
            TimeSpan duration = currentEnd - currentStart;
            
            while ((currentStart <= endDate) && (!maxOccurrences.HasValue || instances.Count < maxOccurrences.Value))
            {
                // Skip the first instance (it's the parent)
                if (instanceNumber > 1)
                {
                    var instance = new DriverSchedule
                    {
                        DriverId = parentSchedule.DriverId,
                        StartTime = currentStart,
                        EndTime = currentStart.Add(duration),
                        Status = ScheduleStatus.Pending,
                        Notes = parentSchedule.Notes,
                        InstanceNumber = instanceNumber
                    };
                    
                    instances.Add(instance);
                }
                
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
                
                instanceNumber++;
            }
            
            return instances;
        }

        /// <summary>
        /// Gets all instances of a recurring schedule
        /// </summary>
        public async Task<IEnumerable<RecurringScheduleInstance>> GetRecurringScheduleInstancesAsync(long parentScheduleId)
        {
            return await _context.Set<RecurringScheduleInstance>()
                .Where(i => i.ParentScheduleId == parentScheduleId)
                .OrderBy(i => i.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Updates a recurring schedule and optionally all its instances
        /// </summary>
        public async Task<bool> UpdateRecurringScheduleAsync(
            DriverSchedule updatedParentSchedule, 
            bool applyToAllInstances)
        {
            var existingParent = await _context.Set<DriverSchedule>()
                .FindAsync(updatedParentSchedule.Id);
                
            if (existingParent == null || !existingParent.IsRecurring)
                return false;
                
            // Update the parent schedule
            _context.Entry(existingParent).CurrentValues.SetValues(updatedParentSchedule);
            existingParent.UpdatedDate = DateTime.UtcNow;
            
            if (applyToAllInstances)
            {
                // Get all instances that haven't been modified
                var instances = await _context.Set<RecurringScheduleInstance>()
                    .Where(i => i.ParentScheduleId == updatedParentSchedule.Id && !i.IsModified)
                    .ToListAsync();
                
                foreach (var instance in instances)
                {
                    // Calculate the time difference between parent start and instance start
                    var timeDiff = instance.StartTime - existingParent.StartTime;
                    
                    // Apply updates while maintaining the schedule pattern
                    instance.StartTime = updatedParentSchedule.StartTime.Add(timeDiff);
                    instance.EndTime = instance.StartTime.Add(updatedParentSchedule.EndTime - updatedParentSchedule.StartTime);
                    instance.Notes = updatedParentSchedule.Notes;
                    instance.UpdatedDate = DateTime.UtcNow;
                }
            }
            
            await _context.SaveChangesAsync();
            
            await _userActivityService.LogActivityAsync(
                existingParent.Driver.UserId,
                ActivityTypes.UpdateSchedule,
                $"Updated recurring schedule {(applyToAllInstances ? "and all instances" : "")}",
                "RecurringSchedule",
                existingParent.Id.ToString());
                
            return true;
        }

        /// <summary>
        /// Deletes a recurring schedule and optionally all its instances
        /// </summary>
        public async Task<bool> DeleteRecurringScheduleAsync(long scheduleId, bool deleteAllInstances)
        {
            var parentSchedule = await _context.Set<DriverSchedule>()
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.IsRecurring);
                
            if (parentSchedule == null)
                return false;
                
            if (deleteAllInstances)
            {
                // Cancel all future instances
                var instances = await _context.Set<RecurringScheduleInstance>()
                    .Where(i => i.ParentScheduleId == scheduleId && i.StartTime > DateTime.UtcNow)
                    .ToListAsync();
                    
                foreach (var instance in instances)
                {
                    instance.Status = ScheduleStatus.Cancelled;
                    instance.UpdatedDate = DateTime.UtcNow;
                }
            }
            
            // Mark parent as cancelled
            parentSchedule.Status = ScheduleStatus.Cancelled;
            parentSchedule.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            await _userActivityService.LogActivityAsync(
                parentSchedule.Driver.UserId,
                ActivityTypes.DeleteSchedule,
                $"Deleted recurring schedule {(deleteAllInstances ? "and all instances" : "")}",
                "RecurringSchedule",
                scheduleId.ToString());
                
            return true;
        }

        /// <summary>
        /// Gets all recurring schedules for a driver
        /// </summary>
        public async Task<IEnumerable<DriverSchedule>> GetDriverRecurringSchedulesAsync(long driverId)
        {
            return await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId && s.IsRecurring && s.Status != ScheduleStatus.Cancelled)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }
} 