using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class DriverDocument
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DriverId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string DocumentUrl { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [MaxLength(255)]
        public string IssuingAuthority { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public DocumentStatus Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Verification details
        public bool IsVerified { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string? VerifiedBy { get; set; }

        [ForeignKey("DriverId")]
        public Driver Driver { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }

    public enum DocumentStatus
    {
        Active,
        Expired,
        PendingRenewal,
        PendingVerification,
        Rejected
    }

    public static class DocumentTypes
    {
        public const string DriversLicense = "Drivers License";
        public const string CDL = "Commercial Drivers License";
        public const string MedicalCertificate = "Medical Certificate";
        public const string SafetyTraining = "Safety Training Certificate";
        public const string HazmatCertification = "Hazmat Certification";
        public const string DefensiveDrivingCertificate = "Defensive Driving Certificate";
        public const string Insurance = "Insurance Document";
        public const string Background_Check = "Background Check";
        public const string DrugTest = "Drug Test Results";
    }
}
