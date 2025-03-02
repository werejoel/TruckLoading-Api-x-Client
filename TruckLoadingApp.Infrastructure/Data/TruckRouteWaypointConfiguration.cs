using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckRouteWaypointConfiguration : IEntityTypeConfiguration<TruckRouteWaypoint>
    {
        public void Configure(EntityTypeBuilder<TruckRouteWaypoint> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.SequenceNumber)
                .IsRequired();

            builder.Property(w => w.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(w => w.Latitude)
                .IsRequired()
                .HasColumnType("decimal(9,6)");

            builder.Property(w => w.Longitude)
                .IsRequired()
                .HasColumnType("decimal(9,6)");

            builder.Property(w => w.Notes)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(w => w.TruckRoute)
                .WithMany(tr => tr.Waypoints)
                .HasForeignKey(w => w.TruckRouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 