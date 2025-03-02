using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckRouteConfiguration : IEntityTypeConfiguration<TruckRoute>
    {
        public void Configure(EntityTypeBuilder<TruckRoute> builder)
        {
            builder.HasKey(tr => tr.Id);

            builder.Property(tr => tr.RouteName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(tr => tr.BasePricePerKm)
                .HasColumnType("decimal(18,2)");

            builder.Property(tr => tr.BasePricePerKg)
                .HasColumnType("decimal(18,2)");

            builder.Property(tr => tr.Currency)
                .HasMaxLength(3)
                .IsRequired();

            // Relationships
            builder.HasOne(tr => tr.Truck)
                .WithMany()
                .HasForeignKey(tr => tr.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tr => tr.Waypoints)
                .WithOne(w => w.TruckRoute)
                .HasForeignKey(w => w.TruckRouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 