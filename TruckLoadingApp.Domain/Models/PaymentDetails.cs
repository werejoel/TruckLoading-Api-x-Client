using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.Models
{
    public class PaymentDetails
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long BookingId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentStatusEnum PaymentStatus { get; set; }

        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Required]
        public Booking Booking { get; set; } = null!; // Navigation property

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public string? PaymentProvider { get; set; }
    }
}
