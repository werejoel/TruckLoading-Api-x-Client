using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class LoadTemperatureRequirementConfiguration : IEntityTypeConfiguration<LoadTemperatureRequirement>
    {
        public void Configure(EntityTypeBuilder<LoadTemperatureRequirement> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.MinTemperature)
                .HasColumnType("decimal(5, 2)")
                .IsRequired();

            builder.Property(t => t.MaxTemperature)
                .HasColumnType("decimal(5, 2)")
                .IsRequired();

            builder.Property(t => t.TemperatureUnit)
                .IsRequired()
                .HasMaxLength(10);

            // One-to-one relationship with Load is configured in LoadConfiguration
        }
    }
}
