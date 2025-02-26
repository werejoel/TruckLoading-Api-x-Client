using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckLocationConfiguration : IEntityTypeConfiguration<TruckLocation>
    {
        public void Configure(EntityTypeBuilder<TruckLocation> builder)
        {
            builder.HasKey(tl => tl.Id);

            builder.Property(tl => tl.CurrentLatitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();

            builder.Property(tl => tl.CurrentLongitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();
        }
    }
}