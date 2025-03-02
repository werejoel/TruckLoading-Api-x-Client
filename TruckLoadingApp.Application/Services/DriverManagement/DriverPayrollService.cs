using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class DriverPayrollService : IDriverPayrollService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;

        public DriverPayrollService(ApplicationDbContext context, IUserActivityService userActivityService)
        {
            _context = context;
            _userActivityService = userActivityService;
        }

        public async Task<PayrollEntry> CreatePayrollEntryAsync(PayrollEntry payrollEntry)
        {
            payrollEntry.CreatedDate = DateTime.UtcNow;
            
            // Calculate total compensation
            payrollEntry.TotalCompensation = payrollEntry.RegularPay +
                                           payrollEntry.OvertimePay +
                                           payrollEntry.PerformanceBonus +
                                           payrollEntry.SafetyBonus +
                                           payrollEntry.OtherBonuses -
                                           payrollEntry.Deductions;

            _context.Add(payrollEntry);
            await _context.SaveChangesAsync();
            
            return payrollEntry;
        }

        public async Task<PayrollEntry?> GetPayrollEntryByIdAsync(long id)
        {
            return await _context.Set<PayrollEntry>()
                .Include(e => e.Driver)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<PayrollEntry>> GetDriverPayrollEntriesAsync(
            long driverId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.Set<PayrollEntry>()
                .Where(e => e.DriverId == driverId &&
                           e.StartDate >= startDate &&
                           e.EndDate <= endDate)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<bool> DeletePayrollEntryAsync(long id)
        {
            var entry = await _context.Set<PayrollEntry>().FindAsync(id);
            if (entry == null)
                return false;

            _context.Set<PayrollEntry>().Remove(entry);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PayrollSummary> GetDriverPayrollSummaryAsync(
            long driverId,
            DateTime startDate,
            DateTime endDate)
        {
            var entries = await _context.Set<PayrollEntry>()
                .Where(e => e.DriverId == driverId &&
                           e.StartDate >= startDate &&
                           e.EndDate <= endDate)
                .ToListAsync();

            var summary = new PayrollSummary
            {
                DriverId = driverId,
                StartDate = startDate,
                EndDate = endDate,
                TotalRegularPay = entries.Sum(e => e.RegularPay),
                TotalOvertimePay = entries.Sum(e => e.OvertimePay),
                TotalPerformanceBonus = entries.Sum(e => e.PerformanceBonus),
                TotalSafetyBonus = entries.Sum(e => e.SafetyBonus),
                TotalOtherBonuses = entries.Sum(e => e.OtherBonuses),
                TotalDeductions = entries.Sum(e => e.Deductions),
                TotalCompensation = entries.Sum(e => e.TotalCompensation),
                PayrollEntryCount = entries.Count
            };

            return summary;
        }

        public async Task<IEnumerable<PayrollPeriod>> GetPayrollPeriodsAsync(
            DateTime startDate,
            DateTime endDate)
        {
            // Generate payroll periods (e.g., bi-weekly)
            var periods = new List<PayrollPeriod>();
            var currentStart = startDate;

            while (currentStart < endDate)
            {
                var periodEnd = currentStart.AddDays(14).AddSeconds(-1); // Bi-weekly periods
                if (periodEnd > endDate)
                    periodEnd = endDate;

                periods.Add(new PayrollPeriod
                {
                    StartDate = currentStart,
                    EndDate = periodEnd,
                    IsClosed = periodEnd < DateTime.Today
                });

                currentStart = periodEnd.AddSeconds(1);
            }

            return periods;
        }

        public async Task<IEnumerable<PayrollEntry>> ProcessPayrollAsync(
            DateTime periodStart,
            DateTime periodEnd)
        {
            // Get all drivers
            var drivers = await _context.Drivers.ToListAsync();
            var payrollEntries = new List<PayrollEntry>();

            foreach (var driver in drivers)
            {
                // Create a payroll entry for each driver for this period
                var entry = new PayrollEntry
                {
                    DriverId = driver.Id,
                    StartDate = periodStart,
                    EndDate = periodEnd,
                    PaymentDate = DateTime.Today,
                    Status = PayrollStatus.Approved,
                    CreatedDate = DateTime.UtcNow
                };

                // Calculate pay based on driver's work during this period
                // This is a simplified example - real implementation would calculate based on
                // hours worked, rates, etc.
                entry.RegularPay = 0; // Calculate based on hours worked
                entry.OvertimePay = 0; // Calculate based on overtime hours
                entry.PerformanceBonus = 0; // Calculate based on performance metrics
                entry.SafetyBonus = 0; // Calculate based on safety record
                entry.OtherBonuses = 0;
                entry.Deductions = 0;
                entry.TotalCompensation = entry.RegularPay + entry.OvertimePay + 
                                        entry.PerformanceBonus + entry.SafetyBonus + 
                                        entry.OtherBonuses - entry.Deductions;

                _context.Add(entry);
                payrollEntries.Add(entry);
            }

            await _context.SaveChangesAsync();
            return payrollEntries;
        }

        public async Task<IEnumerable<PayrollEntry>> GetPayrollHistoryAsync(
            long driverId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.Set<PayrollEntry>()
                .Where(e => e.DriverId == driverId &&
                           e.StartDate >= startDate &&
                           e.EndDate <= endDate)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<bool> UpdatePayrollEntryAsync(PayrollEntry entry)
        {
            var existing = await _context.Set<PayrollEntry>()
                .FindAsync(entry.Id);

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(entry);
            existing.UpdatedDate = DateTime.UtcNow;

            // Recalculate total compensation
            existing.TotalCompensation = existing.RegularPay +
                                       existing.OvertimePay +
                                       existing.PerformanceBonus +
                                       existing.SafetyBonus +
                                       existing.OtherBonuses -
                                       existing.Deductions;

            await _context.SaveChangesAsync();
            
            // Log the activity
            await _userActivityService.LogActivityAsync(
                "Payroll",
                $"Updated payroll entry for driver {entry.DriverId}",
                $"PayrollEntry:{entry.Id}"
            );
            
            return true;
        }
    }
}
