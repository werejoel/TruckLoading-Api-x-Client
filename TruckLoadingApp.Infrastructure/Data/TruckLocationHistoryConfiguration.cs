using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckLocationHistoryConfiguration : IEntityTypeConfiguration<TruckLocationHistory>
    {
        public void Configure(EntityTypeBuilder<TruckLocationHistory> builder)
        {
            builder.HasKey(tlh => tlh.Id);

            builder.Property(tlh => tlh.Latitude)
                .HasColumnType("decimal(18, 15)")
                .IsRequired();

            builder.Property(tlh => tlh.Longitude)
                .HasColumnType("decimal(18, 15)")
                .IsRequired();

            builder.Property(tlh => tlh.Speed)
                .HasColumnType("decimal(10, 2)");

            builder.Property(tlh => tlh.Location)
                .HasColumnType("geography");

            //Relationship
            builder.HasOne(tl => tl.Truck)
               .WithMany()
               .HasForeignKey(tl => tl.TruckId)
               .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict
        }
    }
}