using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.SafetyRating)
                .HasColumnType("decimal(3, 2)");

            // Relationships
            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Truck)
                .WithOne(t => t.AssignedDriver) //Connect to the `AssignedDriver` prop in the truck
                .HasForeignKey<Driver>(d => d.TruckId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}