using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class DriverScheduleConfiguration : IEntityTypeConfiguration<DriverSchedule>
    {
        public void Configure(EntityTypeBuilder<DriverSchedule> builder)
        {
            builder.HasKey(ds => ds.Id);

            // Configure decimal properties with appropriate precision and scale
            builder.Property(ds => ds.DistanceCovered)
                .HasColumnType("decimal(10, 2)");

            builder.Property(ds => ds.FuelUsed)
                .HasColumnType("decimal(10, 2)");

            // Configure relationships - use the existing foreign key property
            builder.HasOne(ds => ds.Driver)
                .WithMany(d => d.Schedules)
                .HasForeignKey(ds => ds.DriverId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(ds => ds.Load)
                .WithMany()
                .HasForeignKey(ds => ds.LoadId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
