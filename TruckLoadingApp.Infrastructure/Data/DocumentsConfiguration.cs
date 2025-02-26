using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class DocumentsConfiguration : IEntityTypeConfiguration<Documents>
    {
        public void Configure(EntityTypeBuilder<Documents> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.EntityType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.DocumentUrl)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}