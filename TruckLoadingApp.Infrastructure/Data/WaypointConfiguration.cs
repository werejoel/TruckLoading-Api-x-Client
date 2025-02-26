using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class WaypointConfiguration : IEntityTypeConfiguration<Waypoint>
    {
        public void Configure(EntityTypeBuilder<Waypoint> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Latitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();

            builder.Property(w => w.Longitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();

            builder.Property(w => w.Location)
                .HasColumnType("geography");

            // Relationship with Route
            builder.HasOne(w => w.Route)
                .WithMany(r => r.Waypoints)
                .HasForeignKey(w => w.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}