using Atrio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atrio.Infrastructure.Persistence.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");
        builder.HasKey(record => record.Id);
        builder.HasIndex(record => new { record.StudentId, record.AttendanceDate }).IsUnique();
        builder.HasIndex(record => new { record.ClassId, record.AttendanceDate });
        builder.HasIndex(record => record.AttendanceDate);

        builder.Property(record => record.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(record => record.Notes).HasMaxLength(500);

        builder.HasOne(record => record.Student)
            .WithMany(student => student.AttendanceRecords)
            .HasForeignKey(record => record.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(record => record.Class)
            .WithMany(classEntity => classEntity.AttendanceRecords)
            .HasForeignKey(record => record.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(record => record.RecordedByUser)
            .WithMany(user => user.RecordedAttendance)
            .HasForeignKey(record => record.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
