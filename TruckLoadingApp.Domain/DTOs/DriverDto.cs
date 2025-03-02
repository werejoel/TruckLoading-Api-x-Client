using System;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Domain.DTOs
{
    public class DriverDto
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? CompanyId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public int? Experience { get; set; }
        public decimal? SafetyRating { get; set; }
        public bool IsAvailable { get; set; }
        public int? TruckId { get; set; }
        public string? TruckNumberPlate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Static method to create DTO from entity
        public static DriverDto FromDriver(Driver driver)
        {
            return new DriverDto
            {
                Id = driver.Id,
                UserId = driver.UserId,
                CompanyId = driver.CompanyId,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiryDate = driver.LicenseExpiryDate,
                Experience = driver.Experience,
                SafetyRating = driver.SafetyRating,
                IsAvailable = driver.IsAvailable,
                TruckId = driver.TruckId,
                TruckNumberPlate = driver.Truck?.NumberPlate,
                CreatedDate = driver.CreatedDate,
                UpdatedDate = driver.UpdatedDate
            };
        }
    }
} 