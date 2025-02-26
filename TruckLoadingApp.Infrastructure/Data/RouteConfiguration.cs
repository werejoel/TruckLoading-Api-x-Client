using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.RouteName)
                .IsRequired();

            // Relationship with Truck
            builder.HasOne(r => r.Truck)
                .WithMany()
                .HasForeignKey(r => r.TruckId);
        }
    }
}