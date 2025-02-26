using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class UserLocationConfiguration : IEntityTypeConfiguration<UserLocation>
    {
        public void Configure(EntityTypeBuilder<UserLocation> builder)
        {
            builder.HasKey(ul => ul.Id);

            builder.Property(ul => ul.Latitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();

            builder.Property(ul => ul.Longitude)
                .HasColumnType("decimal(9, 6)")
                .IsRequired();
        }
    }
}