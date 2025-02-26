using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckTypeConfiguration : IEntityTypeConfiguration<TruckType>
    {
        public void Configure(EntityTypeBuilder<TruckType> builder)
        {
            builder.HasKey(tt => tt.Id);

            builder.Property(tt => tt.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("VARCHAR(100)");

            builder.HasIndex(tt => tt.Name)
                .IsUnique();
        }
    }
}