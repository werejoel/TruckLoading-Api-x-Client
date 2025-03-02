using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;
using RestComplianceStatus = TruckLoadingApp.Application.Services.DriverManagement.Interfaces.RestComplianceStatus;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class DriverComplianceService : IDriverComplianceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;
        private readonly ComplianceCheckerService _complianceChecker;

        public DriverComplianceService(
            ApplicationDbContext context, 
            IUserActivityService userActivityService, 
            ComplianceCheckerService complianceChecker)
        {
            _context = context;
            _userActivityService = userActivityService;
            _complianceChecker = complianceChecker;
        }

        public async Task<DriverRestPeriod> RecordRestPeriodAsync(DriverRestPeriod restPeriod)
        {
            // Validate rest period
            if (restPeriod.StartTime >= restPeriod.EndTime)
                throw new ArgumentException("End time must be after start time");

            // Check if duration meets minimum requirements based on type
            var duration = restPeriod.EndTime - restPeriod.StartTime;
            switch (restPeriod.Type)
            {
                case RestType.DailyRest when duration < TimeSpan.FromHours(RestRegulations.MinimumDailyRest):
                    throw new ArgumentException($"Daily rest must be at least {RestRegulations.MinimumDailyRest} hours");
                case RestType.WeeklyRest when duration < TimeSpan.FromHours(RestRegulations.MinimumWeeklyRest):
                    throw new ArgumentException($"Weekly rest must be at least {RestRegulations.MinimumWeeklyRest} hours");
                case RestType.ShortBreak when duration < TimeSpan.FromMinutes(RestRegulations.MinimumBreakDuration):
                    throw new ArgumentException($"Short break must be at least {RestRegulations.MinimumBreakDuration} minutes");
            }

            _context.Add(restPeriod);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                restPeriod.Driver.UserId,
                ActivityTypes.RecordRestPeriod,
                $"Recorded {restPeriod.Type} rest period",
                "RestPeriod",
                restPeriod.Id.ToString());

            return restPeriod;
        }

        public async Task<DriverRestPeriod?> GetRestPeriodByIdAsync(long restPeriodId)
        {
            return await _context.Set<DriverRestPeriod>()
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.Id == restPeriodId);
        }

        public async Task<IEnumerable<DriverRestPeriod>> GetDriverRestPeriodsAsync(
            long driverId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.Set<DriverRestPeriod>()
                .Where(r => r.DriverId == driverId &&
                           r.StartTime >= startDate &&
                           r.EndTime <= endDate)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<bool> UpdateRestPeriodAsync(DriverRestPeriod restPeriod)
        {
            var existingPeriod = await _context.Set<DriverRestPeriod>()
                .FirstOrDefaultAsync(r => r.Id == restPeriod.Id);

            if (existingPeriod == null)
                return false;

            _context.Entry(existingPeriod).CurrentValues.SetValues(restPeriod);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                restPeriod.Driver.UserId,
                ActivityTypes.UpdateRestPeriod,
                $"Updated rest period from {restPeriod.StartTime} to {restPeriod.EndTime}",
                "RestPeriod",
                restPeriod.Id.ToString());

            return true;
        }

        public async Task<bool> DeleteRestPeriodAsync(long restPeriodId)
        {
            var restPeriod = await _context.Set<DriverRestPeriod>()
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.Id == restPeriodId);

            if (restPeriod == null)
                return false;

            _context.Remove(restPeriod);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                restPeriod.Driver.UserId,
                ActivityTypes.DeleteRestPeriod,
                $"Deleted rest period from {restPeriod.StartTime} to {restPeriod.EndTime}",
                "RestPeriod",
                restPeriodId.ToString());

            return true;
        }

        public async Task<RestComplianceStatus> CheckRestComplianceAsync(long driverId)
        {
            var status = new RestComplianceStatus();
            var now = DateTime.UtcNow;
            var yesterday = now.AddDays(-1);
            var lastWeek = now.AddDays(-7);

            // Get recent driving and rest periods
            var recentRestPeriods = await _context.Set<DriverRestPeriod>()
                .Where(r => r.DriverId == driverId &&
                           r.EndTime >= lastWeek &&
                           r.Status == RestStatus.Completed)
                .ToListAsync();

            var schedules = await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId &&
                           s.EndTime >= lastWeek &&
                           s.Status == ScheduleStatus.Completed)
                .ToListAsync();

            // Calculate total driving and rest times
            status.TotalDrivingTime = TimeSpan.FromTicks((long)schedules.Sum(s => (s.EndTime - s.StartTime).Ticks));
            status.TotalRestTime = TimeSpan.FromTicks((long)recentRestPeriods.Sum(r => (r.EndTime - r.StartTime).Ticks));

            // Check daily rest compliance
            var lastDailyRest = recentRestPeriods
                .Where(r => r.Type == RestType.DailyRest)
                .OrderByDescending(r => r.EndTime)
                .FirstOrDefault();

            if (lastDailyRest == null || lastDailyRest.EndTime < yesterday)
            {
                status.IsCompliant = false;
                status.ComplianceNotes = "Missing daily rest period";
                status.Violations++;
            }

            // Check weekly rest compliance
            var weeklyRest = recentRestPeriods
                .Where(r => r.Type == RestType.WeeklyRest && r.EndTime >= lastWeek)
                .Sum(r => (r.EndTime - r.StartTime).TotalHours);

            if (weeklyRest < RestRegulations.MinimumWeeklyRest)
            {
                status.IsCompliant = false;
                status.ComplianceNotes += "\nInsufficient weekly rest";
                status.Violations++;
            }

            // Calculate next required rest
            var lastRestEnd = recentRestPeriods
                .OrderByDescending(r => r.EndTime)
                .FirstOrDefault()?.EndTime ?? lastWeek;

            var drivingTimeSinceRest = schedules
                .Where(s => s.StartTime >= lastRestEnd)
                .Sum(s => (s.EndTime - s.StartTime).TotalHours);

            if (drivingTimeSinceRest >= RestRegulations.RequiredBreakAfter)
            {
                status.NextRequiredRest = DateTime.UtcNow;
                status.ComplianceNotes += "\nBreak required immediately";
                status.Violations++;
            }
            else
            {
                var timeUntilBreak = TimeSpan.FromHours(RestRegulations.RequiredBreakAfter) - TimeSpan.FromHours(drivingTimeSinceRest);
                status.NextRequiredRest = DateTime.UtcNow.Add(timeUntilBreak);
            }

            return status;
        }

        public async Task<DateTime> GetNextRequiredRestTimeAsync(long driverId)
        {
            var lastRestPeriod = await _context.Set<DriverRestPeriod>()
                .Where(r => r.DriverId == driverId && r.Status == RestStatus.Completed)
                .OrderByDescending(r => r.EndTime)
                .FirstOrDefaultAsync();

            if (lastRestPeriod == null)
                return DateTime.UtcNow; // Rest required immediately

            var drivingTimeSinceRest = await _context.Set<DriverSchedule>()
                .Where(s => s.DriverId == driverId &&
                           s.StartTime >= lastRestPeriod.EndTime &&
                           s.Status != ScheduleStatus.Cancelled)
                .SumAsync(s => (s.EndTime - s.StartTime).TotalHours);

            if (drivingTimeSinceRest >= RestRegulations.RequiredBreakAfter)
                return DateTime.UtcNow; // Rest required immediately

            var timeUntilBreak = TimeSpan.FromHours(RestRegulations.RequiredBreakAfter) - TimeSpan.FromHours(drivingTimeSinceRest);
            return DateTime.UtcNow.Add(timeUntilBreak);
        }

        public async Task<bool> IsDriverAvailableAsync(long driverId, DateTime startTime, DateTime endTime)
        {
            return await _complianceChecker.CheckDriverAvailabilityAsync(driverId, startTime, endTime);
        }

        public async Task<IEnumerable<DriverSchedule>> GetAvailableDriversForTimeSlotAsync(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("End time must be after start time");

            var availableDrivers = await _context.Set<Driver>()
                .Include(d => d.User)
                .Where(d => d.IsAvailable)
                .ToListAsync();

            var result = new List<DriverSchedule>();

            foreach (var driver in availableDrivers)
            {
                // Check if the driver is available for this time slot using the compliance checker
                var isAvailable = await _complianceChecker.CheckDriverAvailabilityAsync(driver.Id, startTime, endTime);
                
                if (isAvailable)
                {
                    result.Add(new DriverSchedule
                    {
                        DriverId = driver.Id,
                        Driver = driver,
                        StartTime = startTime,
                        EndTime = endTime,
                        Status = ScheduleStatus.Pending
                    });
                }
            }

            return result;
        }
    }
}
