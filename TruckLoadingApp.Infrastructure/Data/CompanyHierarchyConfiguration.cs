using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class CompanyHierarchyConfiguration : IEntityTypeConfiguration<CompanyHierarchy>
    {
        public void Configure(EntityTypeBuilder<CompanyHierarchy> builder)
        {
            builder.HasKey(ch => ch.Id);

            builder.HasOne(ch => ch.Company)
                .WithMany()
                .HasForeignKey(ch => ch.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ch => ch.ParentDepartment)
                .WithMany(ch => ch.SubDepartments)
                .HasForeignKey(ch => ch.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ch => ch.DepartmentMembers)
                .WithOne(dm => dm.Department)
                .HasForeignKey(dm => dm.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ch => ch.DepartmentName)
                .IsRequired();

            builder.Property(ch => ch.Description)
                .HasMaxLength(500);

            // Create an index on CompanyId and Level for faster hierarchy traversal
            builder.HasIndex(ch => new { ch.CompanyId, ch.Level });
        }
    }

    public class DepartmentMemberConfiguration : IEntityTypeConfiguration<DepartmentMember>
    {
        public void Configure(EntityTypeBuilder<DepartmentMember> builder)
        {
            builder.HasKey(dm => dm.Id);

            builder.HasOne(dm => dm.Department)
                .WithMany(ch => ch.DepartmentMembers)
                .HasForeignKey(dm => dm.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dm => dm.User)
                .WithMany()
                .HasForeignKey(dm => dm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(dm => dm.Role)
                .IsRequired();

            // Create a unique index to prevent duplicate memberships
            builder.HasIndex(dm => new { dm.DepartmentId, dm.UserId }).IsUnique();
        }
    }
}
