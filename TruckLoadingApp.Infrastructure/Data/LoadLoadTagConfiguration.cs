using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class LoadLoadTagConfiguration : IEntityTypeConfiguration<LoadLoadTag>
    {
        public void Configure(EntityTypeBuilder<LoadLoadTag> builder)
        {
            builder.HasKey(lt => lt.Id);

            builder.HasOne(lt => lt.Load)
                .WithMany(l => l.LoadTags)
                .HasForeignKey(lt => lt.LoadId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lt => lt.LoadTag)
                .WithMany(t => t.LoadTags)
                .HasForeignKey(lt => lt.LoadTagId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create a unique index on the combination of LoadId and LoadTagId
            builder.HasIndex(lt => new { lt.LoadId, lt.LoadTagId }).IsUnique();
        }
    }
}
