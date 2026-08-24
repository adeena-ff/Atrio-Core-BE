using Atrio.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Atrio.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.FullName).HasMaxLength(150).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Classes");
            entity.HasKey(classEntity => classEntity.Id);
            entity.HasIndex(classEntity => classEntity.Code).IsUnique();
            entity.Property(classEntity => classEntity.Name).HasMaxLength(120).IsRequired();
            entity.Property(classEntity => classEntity.Code).HasMaxLength(40).IsRequired();
            entity.Property(classEntity => classEntity.AcademicYear).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(student => student.Id);
            entity.HasIndex(student => student.EnrollmentNumber).IsUnique();
            entity.HasIndex(student => student.Email).IsUnique();
            entity.Property(student => student.FirstName).HasMaxLength(80).IsRequired();
            entity.Property(student => student.LastName).HasMaxLength(80).IsRequired();
            entity.Property(student => student.Email).HasMaxLength(256).IsRequired();
            entity.Property(student => student.EnrollmentNumber).HasMaxLength(40).IsRequired();

            entity.HasOne(student => student.Class)
                .WithMany(classEntity => classEntity.Students)
                .HasForeignKey(student => student.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.ToTable("AttendanceRecords");
            entity.HasKey(record => record.Id);
            entity.HasIndex(record => new { record.StudentId, record.AttendanceDate }).IsUnique();
            entity.Property(record => record.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(record => record.Notes).HasMaxLength(500);

            entity.HasOne(record => record.Student)
                .WithMany(student => student.AttendanceRecords)
                .HasForeignKey(record => record.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(record => record.Class)
                .WithMany(classEntity => classEntity.AttendanceRecords)
                .HasForeignKey(record => record.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(record => record.RecordedByUser)
                .WithMany(user => user.RecordedAttendance)
                .HasForeignKey(record => record.RecordedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
