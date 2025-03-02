using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class DriverRoutePreferenceConfiguration : IEntityTypeConfiguration<DriverRoutePreference>
    {
        public void Configure(EntityTypeBuilder<DriverRoutePreference> builder)
        {
            builder.HasKey(drp => drp.Id);

            // Configure decimal properties with appropriate precision and scale
            builder.Property(drp => drp.MaxPreferredWeight)
                .HasColumnType("decimal(10, 2)");

            builder.Property(drp => drp.MaxWindSpeed)
                .HasColumnType("decimal(5, 2)");

            // Configure relationships - use the existing foreign key property
            builder.HasOne(drp => drp.Driver)
                .WithOne(d => d.RoutePreferences)
                .HasForeignKey<DriverRoutePreference>(drp => drp.DriverId);
        }
    }
}
