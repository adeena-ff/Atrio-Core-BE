using Atrio.Domain.Entities;
using Atrio.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Infrastructure.Persistence;

/// <summary>
/// Creates a deterministic, locally repeatable data set for Atrio demonstrations.
/// The presence of the Sarah Jenkins account is the completion marker, so normal
/// application restarts never duplicate or overwrite a completed demo seed.
/// </summary>
public static class DemoDataSeeder
{
    private const string DemoMarkerEmail = "sarah.jenkins@atrio.com";

    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(user => user.Email == DemoMarkerEmail, cancellationToken)) return;

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.AttendanceRecords.ExecuteDeleteAsync(cancellationToken);
        await db.Students.ExecuteDeleteAsync(cancellationToken);
        await db.Classes.ExecuteDeleteAsync(cancellationToken);
        await db.Users.ExecuteDeleteAsync(cancellationToken);

        var hasher = new PasswordHasher<User>();
        var users = CreateUsers(hasher);
        var usersByEmail = users.ToDictionary(user => user.Email);
        var classes = CreateClasses(usersByEmail);
        var students = CreateStudents(classes);
        var attendance = CreateAttendance(students, usersByEmail);

        db.Users.AddRange(users);
        db.Classes.AddRange(classes);
        db.Students.AddRange(students);
        db.AttendanceRecords.AddRange(attendance);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static List<User> CreateUsers(PasswordHasher<User> hasher)
    {
        var definitions = new[]
        {
            ("System Administrator", "admin@atrio.com", UserRole.Admin, "AdminPassword123!"),
            ("Dr. Sarah Jenkins", "sarah.jenkins@atrio.com", UserRole.Teacher, "Password123!"),
            ("Prof. Marcus Vance", "marcus.vance@atrio.com", UserRole.Teacher, "Password123!"),
            ("Elena Rostova", "elena.rostova@atrio.com", UserRole.Teacher, "Password123!"),
            ("David Chen", "david.chen@atrio.com", UserRole.Teacher, "Password123!"),
            ("Default Teacher Account", "teacher@atrio.com", UserRole.Teacher, "TeacherPassword123!")
        };

        return definitions.Select(definition =>
        {
            var user = new User
            {
                Id = Guid.NewGuid(), FullName = definition.Item1, Email = definition.Item2,
                Role = definition.Item3, IsActive = true, CreatedAtUtc = DateTime.UtcNow
            };
            user.PasswordHash = hasher.HashPassword(user, definition.Item4);
            return user;
        }).ToList();
    }

    private static List<Class> CreateClasses(IReadOnlyDictionary<string, User> users) =>
    [
        new() { Id = Guid.NewGuid(), Code = "CSC-101", Name = "Intro to Computer Science", AcademicYear = "2026", TeacherId = users["sarah.jenkins@atrio.com"].Id },
        new() { Id = Guid.NewGuid(), Code = "CSC-202", Name = "Data Structures & Algorithms", AcademicYear = "2026", TeacherId = users["sarah.jenkins@atrio.com"].Id },
        new() { Id = Guid.NewGuid(), Code = "MTH-301", Name = "Linear Algebra & Calculus", AcademicYear = "2026", TeacherId = users["marcus.vance@atrio.com"].Id },
        new() { Id = Guid.NewGuid(), Code = "PHY-105", Name = "General Physics I", AcademicYear = "2026", TeacherId = users["elena.rostova@atrio.com"].Id },
        new() { Id = Guid.NewGuid(), Code = "HIS-210", Name = "Modern World History", AcademicYear = "2026", TeacherId = users["david.chen@atrio.com"].Id },
        new() { Id = Guid.NewGuid(), Code = "CSC-305", Name = "Database Systems", AcademicYear = "2026", TeacherId = users["teacher@atrio.com"].Id }
    ];

    private static List<Student> CreateStudents(IReadOnlyList<Class> classes)
    {
        var names = new[]
        {
            ("Amelia", "Hart"), ("Benjamin", "Cole"), ("Chloe", "Martin"), ("Daniel", "Reed"), ("Evelyn", "Brooks"),
            ("Farhan", "Khan"), ("Grace", "Murphy"), ("Henry", "Ford"), ("Isla", "Patel"), ("Julian", "Scott"),
            ("Katherine", "Moore"), ("Leo", "Turner"), ("Mia", "Evans"), ("Nathan", "Ross"), ("Olivia", "Price"),
            ("Parker", "Wright"), ("Quinn", "Hughes"), ("Ruby", "Foster"), ("Samuel", "Hayes"), ("Tessa", "Bennett"),
            ("Uma", "Wallace"), ("Victor", "James"), ("Willow", "Davis"), ("Xavier", "Bell"), ("Yasmin", "Ali"),
            ("Zachary", "Ward"), ("Ava", "Morgan"), ("Caleb", "Perry"), ("Lily", "Simmons"), ("Owen", "Bailey")
        };

        return names.Select((name, index) => new Student
        {
            Id = Guid.NewGuid(), FirstName = name.Item1, LastName = name.Item2,
            Email = $"{name.Item1.ToLowerInvariant()}.{name.Item2.ToLowerInvariant()}@students.atrio.com",
            EnrollmentNumber = $"AT-2026-{index + 1:D3}", ClassId = classes[index / 5].Id,
            IsActive = true, CreatedAtUtc = DateTime.UtcNow
        }).ToList();
    }

    private static List<AttendanceRecord> CreateAttendance(IReadOnlyList<Student> students, IReadOnlyDictionary<string, User> users)
    {
        var weekdays = GetPreviousWeekdays(60);
        var teacherByClass = new Dictionary<int, User>
        {
            [0] = users["sarah.jenkins@atrio.com"], [1] = users["sarah.jenkins@atrio.com"],
            [2] = users["marcus.vance@atrio.com"], [3] = users["elena.rostova@atrio.com"],
            [4] = users["david.chen@atrio.com"], [5] = users["teacher@atrio.com"]
        };
        var atRisk = new HashSet<string> { "AT-2026-007", "AT-2026-015", "AT-2026-023" };
        var records = new List<AttendanceRecord>(students.Count * weekdays.Count);

        foreach (var (student, studentIndex) in students.Select((student, studentIndex) => (student, studentIndex)))
        {
            var classIndex = studentIndex / 5;
            foreach (var (date, dayIndex) in weekdays.Select((date, dayIndex) => (date, dayIndex)))
            {
                records.Add(new AttendanceRecord
                {
                    Id = Guid.NewGuid(), StudentId = student.Id, ClassId = student.ClassId,
                    RecordedByUserId = teacherByClass[classIndex].Id, AttendanceDate = date,
                    Status = atRisk.Contains(student.EnrollmentNumber) ? RiskStatus(dayIndex) : NormalStatus(dayIndex),
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-dayIndex)
                });
            }
        }

        return records;
    }

    private static AttendanceStatus RiskStatus(int dayIndex) => dayIndex % 10 < 6 ? AttendanceStatus.Present : AttendanceStatus.Absent;

    private static AttendanceStatus NormalStatus(int dayIndex) => (dayIndex % 20) switch
    {
        < 16 => AttendanceStatus.Present,
        < 18 => AttendanceStatus.Late,
        18 => AttendanceStatus.Excused,
        _ => AttendanceStatus.Absent
    };

    private static List<DateOnly> GetPreviousWeekdays(int count)
    {
        var days = new List<DateOnly>(count);
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        while (days.Count < count)
        {
            if (date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday) days.Add(date);
            date = date.AddDays(-1);
        }
        days.Reverse();
        return days;
    }
}
