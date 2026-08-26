using Atrio.Domain.Entities;
using Atrio.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Atrio.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync(cancellationToken);

        var hasher = new PasswordHasher<User>();
        var teacher = await db.Users.FirstOrDefaultAsync(user => user.Email == "teacher@atrio.com", cancellationToken);
        if (teacher is null)
        {
            var admin = new User { Id = Guid.NewGuid(), FullName = "Atrio Administrator", Email = "admin@atrio.com", Role = UserRole.Admin };
            admin.PasswordHash = hasher.HashPassword(admin, "AdminPassword123!");
            teacher = new User { Id = Guid.NewGuid(), FullName = "Morgan Teacher", Email = "teacher@atrio.com", Role = UserRole.Teacher };
            teacher.PasswordHash = hasher.HashPassword(teacher, "TeacherPassword123!");
            var mathematics = new Class { Id = Guid.NewGuid(), Name = "Mathematics 10A", Code = "MTH-10A", AcademicYear = "2026", IsActive = true };
            var science = new Class { Id = Guid.NewGuid(), Name = "Computer Science 12A", Code = "CSC-12A", AcademicYear = "2026", IsActive = true, TeacherId = teacher.Id };
            db.AddRange(admin, teacher, mathematics, science);
            db.Students.AddRange(
                new Student { Id = Guid.NewGuid(), FirstName = "Maya", LastName = "Chen", Email = "maya.chen@atrio.com", EnrollmentNumber = "AT-2026-001", ClassId = mathematics.Id },
                new Student { Id = Guid.NewGuid(), FirstName = "Noah", LastName = "Williams", Email = "noah.williams@atrio.com", EnrollmentNumber = "AT-2026-002", ClassId = mathematics.Id },
                new Student { Id = Guid.NewGuid(), FirstName = "Liam", LastName = "Johnson", Email = "liam.johnson@atrio.com", EnrollmentNumber = "AT-2026-003", ClassId = science.Id },
                new Student { Id = Guid.NewGuid(), FirstName = "Sophia", LastName = "Brown", Email = "sophia.brown@atrio.com", EnrollmentNumber = "AT-2026-004", ClassId = science.Id });
            await db.SaveChangesAsync(cancellationToken);
            return;
        }

        var computerScience = await db.Classes.FirstOrDefaultAsync(classEntity => classEntity.Code == "CSC-12A", cancellationToken);
        if (computerScience is not null && computerScience.TeacherId != teacher.Id)
        {
            computerScience.TeacherId = teacher.Id;
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
