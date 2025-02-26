using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasKey(re => re.Id);

            builder.Property(re => re.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(re => re.Coordinates)
                .HasMaxLength(500);
        }
    }
}