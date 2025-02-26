using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class LoadStopConfiguration : IEntityTypeConfiguration<LoadStop>
    {
        public void Configure(EntityTypeBuilder<LoadStop> builder)
        {
            builder.HasKey(ls => ls.Id);

            builder.Property(ls => ls.StopLatitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();

            builder.Property(ls => ls.StopLongitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();

            builder.Property(ls => ls.Location)
                .HasColumnType("geography");

            //Relationship
            builder.HasOne(ls => ls.Booking)
            .WithMany()
            .HasForeignKey(ls => ls.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}