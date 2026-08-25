using Atrio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atrio.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(student => student.Id);
        builder.HasIndex(student => student.EnrollmentNumber).IsUnique();
        builder.HasIndex(student => student.Email).IsUnique();
        builder.HasIndex(student => new { student.LastName, student.FirstName });
        builder.HasIndex(student => student.ClassId);

        builder.Property(student => student.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(student => student.LastName).HasMaxLength(80).IsRequired();
        builder.Property(student => student.Email).HasMaxLength(256).IsRequired();
        builder.Property(student => student.EnrollmentNumber).HasMaxLength(40).IsRequired();

        builder.HasOne(student => student.Class)
            .WithMany(classEntity => classEntity.Students)
            .HasForeignKey(student => student.ClassId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
