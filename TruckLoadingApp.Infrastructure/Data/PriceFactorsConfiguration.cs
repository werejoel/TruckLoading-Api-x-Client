using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class PriceFactorsConfiguration : IEntityTypeConfiguration<PriceFactors>
    {
        public void Configure(EntityTypeBuilder<PriceFactors> builder)
        {
            builder.HasKey(pf => pf.Id);

            builder.Property(pf => pf.DistanceRate)
                .HasColumnType("decimal(18, 2)");

            builder.Property(pf => pf.WeightRate)
                .HasColumnType("decimal(18, 2)");

            // Relationships
            builder.HasOne(pf => pf.LoadType)
                .WithMany()
                .HasForeignKey(pf => pf.LoadTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pf => pf.Region)
                .WithMany()
                .HasForeignKey(pf => pf.RegionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}