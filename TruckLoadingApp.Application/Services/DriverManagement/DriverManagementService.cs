using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;
using IDriverManagementService = TruckLoadingApp.Application.Services.DriverManagement.Interfaces.IDriverManagementService;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class DriverManagementService : IDriverManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;
        private readonly IDriverScheduleService _driverScheduleService;
        private readonly IDriverPerformanceService _driverPerformanceService;
        private readonly IDriverDocumentService _driverDocumentService;
        private readonly IDriverComplianceService _driverComplianceService;
        private readonly IDriverPayrollService _driverPayrollService;
        private readonly IDriverRoutePreferenceService _driverRoutePreferenceService; // Inject

        public DriverManagementService(
            ApplicationDbContext context,
            IUserActivityService userActivityService,
            IDriverScheduleService driverScheduleService,
            IDriverPerformanceService driverPerformanceService,
            IDriverDocumentService driverDocumentService,
            IDriverComplianceService driverComplianceService,
            IDriverPayrollService driverPayrollService,
            IDriverRoutePreferenceService driverRoutePreferenceService) //Update Constructor
        {
            _context = context;
            _userActivityService = userActivityService;
            _driverScheduleService = driverScheduleService;
            _driverPerformanceService = driverPerformanceService;
            _driverDocumentService = driverDocumentService;
            _driverComplianceService = driverComplianceService;
            _driverPayrollService = driverPayrollService;
            _driverRoutePreferenceService = driverRoutePreferenceService;
        }


        // Analytics
        public async Task<DriverPerformanceAnalytics> GetDriverAnalyticsAsync(long driverId, DateTime startDate, DateTime endDate)
        {
            var driver = await _context.Drivers
                .Include(d => d.Schedules)
                .Include(d => d.Performances)
                .Include(d => d.RestPeriods)
                .Include(d => d.RoutePreferences)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new KeyNotFoundException($"Driver with ID {driverId} not found");

            var analytics = new DriverPerformanceAnalytics
            {
                DriverId = driverId,
                Period = new DateRange { StartDate = startDate, EndDate = endDate }
            };

            // Get all completed schedules within date range
            var schedules = driver.Schedules
                .Where(s => s.StartTime >= startDate &&
                           s.EndTime <= endDate &&
                           s.Status == ScheduleStatus.Completed)
                .ToList();

            // Get all performance records within date range
            var performances = driver.Performances
                .Where(p => p.Date >= startDate &&
                           p.Date <= endDate)
                .ToList();

            // Calculate basic metrics
            analytics.TotalDeliveries = schedules.Count;
            analytics.TotalDrivingTime = TimeSpan.FromHours(Convert.ToDouble(schedules.Sum(s => (s.EndTime - s.StartTime).TotalHours)));
            analytics.AverageRating = performances.Any() ? performances.Average(p => p.Rating) : 0;

            // Calculate on-time delivery rate
            var onTimeDeliveries = schedules.Count(s => !s.Load?.IsLate ?? false);
            analytics.OnTimeDeliveryRate = analytics.TotalDeliveries > 0
                ? (decimal)onTimeDeliveries / analytics.TotalDeliveries
                : 0;

            // Calculate fuel efficiency
            if (schedules.Any())
            {
                var schedulesWithFuel = schedules.Where(s => s.FuelUsed > 0 && s.DistanceCovered > 0).ToList();
                if (schedulesWithFuel.Any())
                {
                    var totalFuel = schedulesWithFuel.Sum(s => s.FuelUsed);
                    var totalDistance = schedulesWithFuel.Sum(s => s.DistanceCovered);
                    analytics.AverageFuelEfficiency = totalDistance / totalFuel;
                }
            }

            // Calculate safety metrics
            analytics.SafetyIncidents = performances.Sum(p => p.SafetyViolations);
            analytics.SafetyScore = _driverPerformanceService.CalculateSafetyScore(performances);

            // Get rest compliance
            var restPeriods = driver.RestPeriods
                .Where(r => r.StartTime >= startDate &&
                           r.EndTime <= endDate)
                .ToList();

            analytics.RestComplianceRate = CalculateRestComplianceRate(restPeriods, startDate, endDate);

            // Calculate route preferences alignment
            if (driver.RoutePreferences != null)
            {
                analytics.RoutePreferenceCompliance = await CalculateRoutePreferenceComplianceAsync(
                    driver.RoutePreferences,
                    schedules.Where(s => s.Load != null).Select(s => s.Load!).ToList()
                );
            }

            return analytics;
        }

        public async Task<TeamPerformanceReport> GetTeamPerformanceReportAsync(
            IEnumerable<long> driverIds,
            DateTime startDate,
            DateTime endDate)
        {
            var report = new TeamPerformanceReport
            {
                Period = new DateRange { StartDate = startDate, EndDate = endDate },
                TeamSize = driverIds.Count()
            };

            var allAnalytics = new List<DriverPerformanceAnalytics>();
            foreach (var driverId in driverIds)
            {
                var driverAnalytics = await GetDriverAnalyticsAsync(driverId, startDate, endDate);
                allAnalytics.Add(driverAnalytics);
            }

            // Calculate team averages
            report.AverageDeliveries = (decimal)allAnalytics.Average(a => a.TotalDeliveries);
            report.AverageRating = (decimal)allAnalytics.Average(a => a.AverageRating);
            report.AverageOnTimeDeliveryRate = allAnalytics.Average(a => a.OnTimeDeliveryRate);
            report.AverageSafetyScore = allAnalytics.Average(a => a.SafetyScore);
            report.AverageRestComplianceRate = allAnalytics.Average(a => a.RestComplianceRate);

            // Identify top performers
            report.TopPerformers = allAnalytics
                .OrderByDescending(a => a.AverageRating)
                .Take(3)
                .Select(a => new TopPerformer
                {
                    DriverId = a.DriverId,
                    Rating = (decimal)a.AverageRating,
                    Deliveries = a.TotalDeliveries,
                    OnTimeRate = a.OnTimeDeliveryRate
                })
                .ToList();

            // Calculate safety statistics
            report.TotalSafetyIncidents = allAnalytics.Sum(a => a.SafetyIncidents);
            report.SafetyIncidentsPerDriver = (decimal)report.TotalSafetyIncidents / report.TeamSize;

            // Identify areas for improvement
            report.ImprovementAreas = IdentifyTeamImprovementAreas(allAnalytics)
                .Select(area => area.Area)
                .ToList();

            return report;
        }

        private decimal CalculateRestComplianceRate(
            IEnumerable<DriverRestPeriod> restPeriods,
            DateTime startDate,
            DateTime endDate)
        {
            var totalWorkDays = (endDate - startDate).Days;
            var requiredRests = totalWorkDays / RestRegulations.DaysBetweenRequiredRests;
            var actualRests = restPeriods.Count();

            return requiredRests > 0 ? (decimal)actualRests / requiredRests : 1.0m;
        }

        private async Task<decimal> CalculateRoutePreferenceComplianceAsync(
            DriverRoutePreference routePreferences,
            List<Load> loads)
        {
            var complianceScore = 0.0m;
            var totalFactors = 0;

            foreach (var load in loads)
            {
                if (load.Region != null && routePreferences.PreferredRegions != null)
                {
                    totalFactors++;
                    if (routePreferences.PreferredRegions.Contains(load.Region))
                        complianceScore++;
                }

                if (load.LoadType != null && routePreferences.PreferredLoadTypes != null)
                {
                    totalFactors++;
                    if (routePreferences.PreferredLoadTypes.Split(',').Contains(load.LoadType.Id.ToString()))

                        complianceScore++;
                }

                if (load.Weight > 0 && routePreferences.MaxPreferredWeight > 0)
                {
                    totalFactors++;
                    if (load.Weight <= routePreferences.MaxPreferredWeight)
                        complianceScore++;
                }
            }

            return totalFactors > 0 ? complianceScore / totalFactors : 1.0m;
        }

        private bool IsNightDriving(DateTime start, DateTime end)
        {
            const int nightStartHour = 22;
            const int nightEndHour = 6;

            var duration = end - start;
            var nightHours = 0.0;

            for (var time = start; time < end; time = time.AddHours(1))
            {
                if (time.Hour >= nightStartHour || time.Hour < nightEndHour)
                    nightHours++;
            }

            return nightHours / duration.TotalHours > 0.5; // More than 50% night driving
        }


        private List<ImprovementArea> IdentifyTeamImprovementAreas(
            IEnumerable<DriverPerformanceAnalytics> analytics)
        {
            var areas = new List<ImprovementArea>();
            const decimal threshold = 0.8m; // 80% threshold for identifying improvement areas

            // Check on-time delivery rate
            var avgOnTimeRate = (decimal)analytics.Average(a => a.OnTimeDeliveryRate);
            if (avgOnTimeRate < threshold)
            {
                areas.Add(new ImprovementArea
                {
                    Area = "On-Time Delivery",
                    CurrentScore = avgOnTimeRate,
                    TargetScore = threshold,
                    Recommendations = new List<string>
                    {
                        "Review route planning procedures",
                        "Implement better traffic monitoring",
                        "Add buffer time for high-risk deliveries"
                    }
                });
            }

            // Check safety score
            var avgSafetyScore = (decimal)analytics.Average(a => a.SafetyScore);
            if (avgSafetyScore < threshold)
            {
                areas.Add(new ImprovementArea
                {
                    Area = "Safety",
                    CurrentScore = avgSafetyScore,
                    TargetScore = threshold,
                    Recommendations = new List<string>
                    {
                        "Schedule additional safety training",
                        "Review common violation patterns",
                        "Implement safety incentive program"
                    }
                });
            }

            // Check rest compliance
            var avgRestCompliance = (decimal)analytics.Average(a => a.RestComplianceRate);
            if (avgRestCompliance < threshold)
            {
                areas.Add(new ImprovementArea
                {
                    Area = "Rest Compliance",
                    CurrentScore = avgRestCompliance,
                    TargetScore = threshold,
                    Recommendations = new List<string>
                    {
                        "Review scheduling practices",
                        "Implement automated rest period alerts",
                        "Provide better rest facilities"
                    }
                });
            }

            return areas;
        }

        // Training and Certification Management
        public async Task<DriverTraining> AssignTrainingAsync(DriverTraining training)
        {
            // Validate training assignment
            if (training.StartDate >= training.EndDate)
                throw new ArgumentException("End date must be after start date");

            if (training.StartDate < DateTime.UtcNow)
                throw new ArgumentException("Training cannot start in the past");

            // Check for other training conflicts
            var hasTrainingConflict = await _context.Set<DriverTraining>()
                .AnyAsync(t => t.DriverId == training.DriverId &&
                              t.Status != TrainingStatus.Completed &&
                              t.Status != TrainingStatus.Cancelled &&
                              ((t.StartDate <= training.StartDate && t.EndDate > training.StartDate) ||
                               (t.StartDate < training.EndDate && t.EndDate >= training.EndDate)));

            if (hasTrainingConflict)
                throw new InvalidOperationException("Training period conflicts with existing training");

            _context.Add(training);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                training.Driver.UserId,
                ActivityTypes.AssignTraining,
                $"Assigned {training.TrainingType} training",
                "Training",
                training.Id.ToString());

            return training;
        }

        public async Task<DriverTraining?> GetTrainingByIdAsync(long trainingId)
        {
            return await _context.Set<DriverTraining>()
                .Include(t => t.Driver)
                .Include(t => t.Instructor)
                .FirstOrDefaultAsync(t => t.Id == trainingId);
        }

        public async Task<IEnumerable<DriverTraining>> GetDriverTrainingHistoryAsync(
            long driverId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Set<DriverTraining>()
                .Include(t => t.Driver)
                .Include(t => t.Instructor)
                .Where(t => t.DriverId == driverId);

            if (startDate.HasValue)
                query = query.Where(t => t.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.EndDate <= endDate.Value);

            return await query
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateTrainingStatusAsync(
            long trainingId,
            TrainingStatus newStatus,
            string? completionNotes = null,
            int? score = null)
        {
            var training = await _context.Set<DriverTraining>()
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(t => t.Id == trainingId);

            if (training == null)
                return false;

            // Validate status transition
            if (!IsValidStatusTransition(training.Status, newStatus))
                throw new InvalidOperationException($"Invalid status transition from {training.Status} to {newStatus}");

            training.Status = newStatus;
            training.CompletionNotes = completionNotes;
            training.Score = score;
            training.UpdatedDate = DateTime.UtcNow;

            if (newStatus == TrainingStatus.Completed)
            {
                training.CompletionDate = DateTime.UtcNow;

                // Update driver certifications if applicable
                if (training.UpdatesCertification)
                {
                    await UpdateDriverCertificationAsync(training);
                }
            }

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                training.Driver.UserId,
                ActivityTypes.UpdateTraining,
                $"Updated {training.TrainingType} training status to {newStatus}",
                "Training",
                trainingId.ToString());

            return true;
        }

        private bool IsValidStatusTransition(TrainingStatus currentStatus, TrainingStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (TrainingStatus.Scheduled, TrainingStatus.InProgress) => true,
                (TrainingStatus.Scheduled, TrainingStatus.Cancelled) => true,
                (TrainingStatus.InProgress, TrainingStatus.Completed) => true,
                (TrainingStatus.InProgress, TrainingStatus.Failed) => true,
                _ => false
            };
        }

        private async Task UpdateDriverCertificationAsync(DriverTraining training)
        {
            var certification = await _context.Set<DriverCertification>()
                .FirstOrDefaultAsync(c => c.DriverId == training.DriverId &&
                                        c.CertificationType == training.CertificationType);

            if (certification == null)
            {
                // Create new certification
                certification = new DriverCertification
                {
                    DriverId = training.DriverId,
                    CertificationType = training.CertificationType,
                    IssueDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(training.CertificationValidityMonths ?? 12),
                    IssuedBy = training.Instructor?.FullName ?? "System",
                    Status = CertificationStatus.Active
                };
                _context.Add(certification);
            }
            else
            {
                // Update existing certification
                certification.RenewalDate = DateTime.UtcNow;
                certification.ExpiryDate = DateTime.UtcNow.AddMonths(training.CertificationValidityMonths ?? 12);
                certification.Status = CertificationStatus.Active;
            }

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                training.Driver.UserId,
                ActivityTypes.UpdateCertification,
                $"Updated {training.TrainingType} certification",
                "Certification",
                certification.Id.ToString());
        }

        public async Task<IEnumerable<DriverCertification>> GetDriverCertificationsAsync(long driverId)
        {
            return await _context.Set<DriverCertification>()
                .Where(c => c.DriverId == driverId)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DriverCertification>> GetExpiringCertificationsAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _context.Set<DriverCertification>()
                .Include(c => c.Driver)
                .Where(c => c.ExpiryDate <= thresholdDate &&
                           c.Status == CertificationStatus.Active)
                .OrderBy(c => c.ExpiryDate)
                .ToListAsync();
        }

        public async Task<TrainingRequirements> GetDriverTrainingRequirementsAsync(long driverId)
        {
            var driver = await _context.Set<Driver>()
                .Include(d => d.Certifications)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new ArgumentException("Driver not found");

            var requirements = new TrainingRequirements
            {
                DriverId = driverId,
                RequiredTrainings = new List<RequiredTraining>()
            };

            // Check certifications
            foreach (var cert in driver.Certifications)
            {
                if (cert.Status != CertificationStatus.Active ||
                    cert.ExpiryDate <= DateTime.UtcNow.AddMonths(3))
                {
                    requirements.RequiredTrainings.Add(new RequiredTraining
                    {
                        TrainingType = $"{cert.CertificationType} Renewal",
                        Priority = cert.ExpiryDate <= DateTime.UtcNow ?
                            TrainingPriority.Critical :
                            TrainingPriority.High,
                        DueDate = cert.ExpiryDate.ToString("yyyy-MM-dd"),
                        Reason = "Certification expiring or expired"
                    });
                }
            }

            // Check safety violations
            var recentViolations = await _context.Set<DriverPerformance>()
                .Where(p => p.DriverId == driverId &&
                           p.Date >= DateTime.UtcNow.AddMonths(-3) &&
                           p.SafetyViolations > 0)
                .ToListAsync();

            if (recentViolations.Any())
            {
                requirements.RequiredTrainings.Add(new RequiredTraining
                {
                    TrainingType = "Safety Refresher",
                    Priority = TrainingPriority.High,
                    DueDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd"),
                    Reason = "Recent safety violations"
                });
            }

            // Check performance metrics
            var avgRating = await _context.Set<DriverPerformance>()
                .Where(p => p.DriverId == driverId &&
                           p.Date >= DateTime.UtcNow.AddMonths(-3))
                .AverageAsync(p => p.Rating);

            if (avgRating < 3.5m) // Below acceptable threshold
            {
                requirements.RequiredTrainings.Add(new RequiredTraining
                {
                    TrainingType = "Performance Improvement",
                    Priority = TrainingPriority.Medium,
                    DueDate = DateTime.UtcNow.AddDays(60).ToString("yyyy-MM-dd"),
                    Reason = "Below average performance rating"
                });
            }

            return requirements;
        }

        // Document Management
        public async Task<DriverDocument> AddDocumentAsync(DriverDocument document)
        {
            // Validate document
            if (document.ExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Document is already expired");

            _context.Add(document);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                document.Driver.UserId,
                ActivityTypes.AddDocument,
                $"Added {document.DocumentType} document",
                "Document",
                document.Id.ToString());

            return document;
        }

        public async Task<DriverDocument?> GetDocumentByIdAsync(long documentId)
        {
            return await _context.Set<DriverDocument>()
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(d => d.Id == documentId);
        }

        public async Task<IEnumerable<DriverDocument>> GetDriverDocumentsAsync(long driverId)
        {
            return await _context.Set<DriverDocument>()
                .Where(d => d.DriverId == driverId)
                .OrderByDescending(d => d.ExpiryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DriverDocument>> GetExpiringDocumentsAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _context.Set<DriverDocument>()
                .Include(d => d.Driver)
                .Where(d => d.ExpiryDate <= thresholdDate &&
                           d.Status != DocumentStatus.Expired &&
                           d.Status != DocumentStatus.Rejected)
                .OrderBy(d => d.ExpiryDate)
                .ToListAsync();
        }

        public async Task<bool> VerifyDocumentAsync(long documentId, string verifiedBy)
        {
            var document = await _context.Set<DriverDocument>()
                .FindAsync(documentId);

            if (document == null)
                return false;

            document.IsVerified = true;
            document.VerificationDate = DateTime.UtcNow;
            document.VerifiedBy = verifiedBy;
            document.Status = DocumentStatus.Active;

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                verifiedBy,
                ActivityTypes.VerifyDocument,
                $"Verified {document.DocumentType} document",
                "Document",
                documentId.ToString());

            return true;
        }

        public async Task<bool> UpdateDocumentAsync(DriverDocument document)
        {
            var existingDocument = await _context.Set<DriverDocument>()
                .FirstOrDefaultAsync(d => d.Id == document.Id);

            if (existingDocument == null)
                return false;

            _context.Entry(existingDocument).CurrentValues.SetValues(document);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                document.Driver.UserId,
                ActivityTypes.UpdateDocument,
                $"Updated document {document.DocumentType}",
                "Document",
                document.Id.ToString());

            return true;
        }

        public async Task<bool> DeleteDocumentAsync(long documentId)
        {
            var document = await _context.Set<DriverDocument>()
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
                return false;

            _context.Remove(document);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                document.Driver.UserId,
                ActivityTypes.DeleteDocument,
                $"Deleted document {document.DocumentType}",
                "Document",
                documentId.ToString());

            return true;
        }
    }
}
