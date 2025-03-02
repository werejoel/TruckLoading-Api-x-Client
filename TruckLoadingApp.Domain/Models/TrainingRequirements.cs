namespace TruckLoadingApp.Domain.Models
{
    public class TrainingRequirements
    {
        public long DriverId { get; set; }
        public List<RequiredTraining> RequiredTrainings { get; set; } = new();
        public string Notes { get; set; } = string.Empty;

        // Navigation property
        public virtual Driver Driver { get; set; } = null!;
    }

    public class RequiredTraining
    {
        public string TrainingType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public TrainingPriority Priority { get; set; }
        public string? DueDate { get; set; }
        public string? Notes { get; set; }
    }
}
