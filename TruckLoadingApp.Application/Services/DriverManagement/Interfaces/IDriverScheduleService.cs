using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverScheduleService
    {
        Task<DriverSchedule> CreateScheduleAsync(DriverSchedule schedule);
        Task<DriverSchedule?> GetScheduleByIdAsync(long scheduleId);
        Task<IEnumerable<DriverSchedule>> GetDriverSchedulesAsync(long driverId, DateTime startDate, DateTime endDate);
        Task<bool> UpdateScheduleAsync(DriverSchedule schedule);
        Task<bool> DeleteScheduleAsync(long scheduleId);
        Task<IEnumerable<DriverSchedule>> GetAvailableDriversForTimeSlotAsync(DateTime startTime, DateTime endTime);
        Task<bool> IsDriverAvailableAsync(long driverId, DateTime startTime, DateTime endTime);
        
        // Recurring schedule methods
        Task<IEnumerable<DriverSchedule>> CreateRecurringScheduleAsync(DriverSchedule schedule, RecurrencePattern pattern, DateTime endDate, int? maxOccurrences = null);
        Task<IEnumerable<DriverSchedule>> GetRecurringScheduleInstancesAsync(long recurringScheduleId);
        Task<bool> UpdateRecurringScheduleAsync(DriverSchedule schedule, bool applyToAllInstances);
        Task<bool> DeleteRecurringScheduleAsync(long scheduleId, bool deleteAllInstances);
        Task<IEnumerable<DriverSchedule>> GetDriverRecurringSchedulesAsync(long driverId);
    }
}
