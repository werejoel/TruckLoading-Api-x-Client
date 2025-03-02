using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverPayrollService
    {
        Task<PayrollEntry> CreatePayrollEntryAsync(PayrollEntry payrollEntry);
        Task<PayrollEntry?> GetPayrollEntryByIdAsync(long id);
        Task<IEnumerable<PayrollEntry>> GetDriverPayrollEntriesAsync(long driverId, DateTime startDate, DateTime endDate);
        Task<bool> UpdatePayrollEntryAsync(PayrollEntry entry);
        Task<bool> DeletePayrollEntryAsync(long id);
        Task<PayrollSummary> GetDriverPayrollSummaryAsync(long driverId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<PayrollPeriod>> GetPayrollPeriodsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PayrollEntry>> ProcessPayrollAsync(DateTime periodStart, DateTime periodEnd);
        Task<IEnumerable<PayrollEntry>> GetPayrollHistoryAsync(long driverId, DateTime startDate, DateTime endDate);
    }
}
