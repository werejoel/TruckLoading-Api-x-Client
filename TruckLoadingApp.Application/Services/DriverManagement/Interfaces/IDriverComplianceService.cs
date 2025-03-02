using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverComplianceService
    {
        Task<DriverRestPeriod> RecordRestPeriodAsync(DriverRestPeriod restPeriod);
        Task<DriverRestPeriod?> GetRestPeriodByIdAsync(long restPeriodId);
        Task<IEnumerable<DriverRestPeriod>> GetDriverRestPeriodsAsync(long driverId, DateTime startDate, DateTime endDate);
        Task<bool> UpdateRestPeriodAsync(DriverRestPeriod restPeriod);
        Task<bool> DeleteRestPeriodAsync(long restPeriodId);
        Task<RestComplianceStatus> CheckRestComplianceAsync(long driverId);
        Task<DateTime> GetNextRequiredRestTimeAsync(long driverId);
        Task<bool> IsDriverAvailableAsync(long driverId, DateTime startTime, DateTime endTime);
        Task<IEnumerable<DriverSchedule>> GetAvailableDriversForTimeSlotAsync(DateTime startTime, DateTime endTime);
    }
}
