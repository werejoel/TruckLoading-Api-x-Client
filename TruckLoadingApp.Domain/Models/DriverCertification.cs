namespace TruckLoadingApp.Domain.Models
{
    public class DriverCertification
    {
        public long Id { get; set; }
        public long DriverId { get; set; }
        public string CertificationType { get; set; } = string.Empty;
        public string CertificationNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? RenewalDate { get; set; }
        public CertificationStatus Status { get; set; }
        public string? Notes { get; set; }

        // Navigation property
        public virtual Driver Driver { get; set; } = null!;
    }
}
