using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class PayrollEntry
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DriverId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal RegularHours { get; set; }

        public decimal OvertimeHours { get; set; }

        public decimal RegularRate { get; set; }

        public decimal OvertimeRate { get; set; }

        public decimal RegularPay { get; set; }

        public decimal OvertimePay { get; set; }

        public decimal PerformanceBonus { get; set; }

        public decimal SafetyBonus { get; set; }

        public decimal OtherBonuses { get; set; }

        public decimal Deductions { get; set; }

        public decimal TotalCompensation { get; set; }

        public decimal TotalPay { get; set; }

        public string? Notes { get; set; }

        public PayrollStatus Status { get; set; }

        [ForeignKey("DriverId")]
        public Driver Driver { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
    }

    public enum PayrollStatus
    {
        Draft,
        Pending,
        Approved,
        Paid,
        Cancelled
    }

    public static class PayrollConstants
    {
        public const decimal StandardWeeklyHours = 40m;
        public const decimal OvertimeMultiplier = 1.5m;
        public const decimal ExcellentPerformanceRate = 2.0m;
        public const decimal GoodPerformanceRate = 1.5m;
        public const decimal SatisfactoryPerformanceRate = 1.0m;
    }
}
