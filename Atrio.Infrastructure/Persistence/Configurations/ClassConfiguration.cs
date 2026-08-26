using Atrio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atrio.Infrastructure.Persistence.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Classes");
        builder.HasKey(classEntity => classEntity.Id);
        builder.HasIndex(classEntity => classEntity.Code).IsUnique();
        builder.Property(classEntity => classEntity.Name).HasMaxLength(120).IsRequired();
        builder.Property(classEntity => classEntity.Code).HasMaxLength(40).IsRequired();
        builder.Property(classEntity => classEntity.AcademicYear).HasMaxLength(20).IsRequired();
        builder.HasIndex(classEntity => classEntity.IsActive);
        builder.HasIndex(classEntity => classEntity.TeacherId);
        builder.HasOne(classEntity => classEntity.Teacher).WithMany(user => user.AssignedClasses).HasForeignKey(classEntity => classEntity.TeacherId).OnDelete(DeleteBehavior.SetNull);
    }
}
