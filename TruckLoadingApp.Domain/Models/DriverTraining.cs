namespace TruckLoadingApp.Domain.Models
{
    public class DriverTraining
    {
        public long Id { get; set; }
        public long DriverId { get; set; }
        public long? InstructorId { get; set; }
        public string TrainingType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TrainingStatus Status { get; set; }
        public string? CompletionNotes { get; set; }
        public double? Score { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool UpdatesCertification { get; set; }
        public string? CertificationType { get; set; }
        public int? CertificationValidityMonths { get; set; }

        // Navigation properties
        public virtual Driver Driver { get; set; } = null!;
        public virtual Driver? Instructor { get; set; }
    }
}
