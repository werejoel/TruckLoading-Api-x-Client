using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckConfiguration : IEntityTypeConfiguration<Truck>
    {
        public void Configure(EntityTypeBuilder<Truck> builder)
        {
            builder.HasKey(t => t.Id); // Explicitly set primary key

            builder.Property(t => t.NumberPlate)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.LoadCapacityWeight)
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.LoadCapacityVolume)
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Height)
                .HasColumnType("decimal(18, 2)");

            builder.Property(t => t.Width)
                .HasColumnType("decimal(18, 2)");

            builder.Property(t => t.Length)
                .HasColumnType("decimal(18, 2)");

            builder.Property(t => t.AvailableCapacityWeight)
                .HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasOne(t => t.TruckType)
                   .WithMany()
                   .HasForeignKey(t => t.TruckTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Owner)
                   .WithMany(u => u.Trucks)
                   .HasForeignKey(t => t.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Define the relationship with Driver properly
            builder.HasOne(t => t.AssignedDriver)
                   .WithOne()
                   .HasForeignKey<Truck>(t => t.AssignedDriverId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}