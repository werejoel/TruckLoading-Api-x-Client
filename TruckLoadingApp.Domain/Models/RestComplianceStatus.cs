using System;

namespace TruckLoadingApp.Domain.Models
{
    public class RestComplianceStatus
    {
        public bool IsCompliant { get; set; }
        public DateTime? LastRestPeriod { get; set; }
        public TimeSpan? TimeSinceLastRest { get; set; }
        public TimeSpan? MaximumAllowedDrivingTime { get; set; }
        public TimeSpan? RemainingDrivingTime { get; set; }
        public string ComplianceMessage { get; set; }
        public int RequiredRestDuration { get; set; } // In minutes
        public DateTime NextRequiredRestTime { get; set; }
    }
} 