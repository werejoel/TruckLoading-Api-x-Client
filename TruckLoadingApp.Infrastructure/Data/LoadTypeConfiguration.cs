using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class LoadTypeConfiguration : IEntityTypeConfiguration<LoadType>
    {
        public void Configure(EntityTypeBuilder<LoadType> builder)
        {
            builder.HasKey(lt => lt.Id);

            builder.Property(lt => lt.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(lt => lt.Description)
                .HasMaxLength(500);
        }
    }
}