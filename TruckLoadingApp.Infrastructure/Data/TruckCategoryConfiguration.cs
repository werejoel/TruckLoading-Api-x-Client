using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckCategoryConfiguration : IEntityTypeConfiguration<TruckCategory>
    {
        public void Configure(EntityTypeBuilder<TruckCategory> builder)
        {
            builder.HasKey(tc => tc.Id);

            builder.Property(tc => tc.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            // Create a unique index on CategoryName
            builder.HasIndex(tc => tc.CategoryName)
                .IsUnique();

            // Configure the relationship with TruckTypes
            builder.HasMany(tc => tc.TruckTypes)
                .WithOne(tt => tt.Category)
                .HasForeignKey(tt => tt.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 