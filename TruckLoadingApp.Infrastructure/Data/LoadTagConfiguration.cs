using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class LoadTagConfiguration : IEntityTypeConfiguration<LoadTag>
    {
        public void Configure(EntityTypeBuilder<LoadTag> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .HasMaxLength(200);

            // Create a unique index on the Name field
            builder.HasIndex(t => t.Name)
                .IsUnique();
        }
    }
}
