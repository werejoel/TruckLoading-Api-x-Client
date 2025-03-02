using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class DriverPerformanceConfiguration : IEntityTypeConfiguration<DriverPerformance>
    {
        public void Configure(EntityTypeBuilder<DriverPerformance> builder)
        {
            builder.HasKey(dp => dp.Id);

            // Configure decimal properties with appropriate precision and scale
            builder.Property(dp => dp.OnTimeDeliveryRate)
                .HasColumnType("decimal(5, 2)");

            builder.Property(dp => dp.CustomerRating)
                .HasColumnType("decimal(5, 2)");

            builder.Property(dp => dp.FuelEfficiency)
                .HasColumnType("decimal(8, 2)");

            builder.Property(dp => dp.OverallPerformanceScore)
                .HasColumnType("decimal(5, 2)");

            builder.Property(dp => dp.Rating)
                .HasColumnType("decimal(5, 2)");

            builder.Property(dp => dp.SafetyScore)
                .HasColumnType("decimal(5, 2)");

            // Configure relationships - use the existing foreign key property
            builder.HasOne(dp => dp.Driver)
                .WithMany(d => d.Performances)
                .HasForeignKey(dp => dp.DriverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
