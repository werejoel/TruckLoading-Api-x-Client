using System;

namespace TruckLoadingApp.Domain.Models
{
    public class PayrollSummary
    {
        public long DriverId { get; set; }
        public Driver Driver { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRegularPay { get; set; }
        public decimal TotalOvertimePay { get; set; }
        public decimal TotalPerformanceBonus { get; set; }
        public decimal TotalSafetyBonus { get; set; }
        public decimal TotalOtherBonuses { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalCompensation { get; set; }
        public int PayrollEntryCount { get; set; }
    }
} 