using Atrio.Application.Abstractions;
using Atrio.Application.Common;
using Atrio.Application.Common.Models;
using Atrio.Application.Common.Querying;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Application.Mapping;
using Atrio.Domain.Entities;
using Atrio.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Services;

public class StudentService(IApplicationDbContext dbContext) : IStudentService
{
    public async Task<PagedResponse<StudentDto>> SearchAsync(StudentSearchQuery query, CancellationToken cancellationToken = default)
    {
        var studentsQuery = dbContext.Students
            .Include(student => student.Class)
            .Include(student => student.AttendanceRecords)
            .AsQueryable();

        // Security scope is deliberately first so every subsequent filter and count is teacher-safe.
        if (query.TeacherId.HasValue)
        {
            studentsQuery = studentsQuery.Where(student => student.Class.TeacherId == query.TeacherId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            studentsQuery = studentsQuery.Where(student =>
                student.FirstName.ToLower().Contains(term) ||
                student.LastName.ToLower().Contains(term) ||
                student.EnrollmentNumber.ToLower().Contains(term) ||
                student.Email.ToLower().Contains(term));
        }

        if (query.ClassId.HasValue)
        {
            studentsQuery = studentsQuery.Where(student => student.ClassId == query.ClassId.Value);
        }

        var departmentCode = QueryFilter.DepartmentCode(query.Department);
        if (departmentCode is not null)
        {
            studentsQuery = studentsQuery.Where(student => student.Class.Code.StartsWith(departmentCode + "-"));
        }

        var (pageNumber, pageSize) = QueryFilter.NormalizePage(query.PageNumber, query.PageSize);
        var totalCount = await studentsQuery.CountAsync(cancellationToken);
        var students = await studentsQuery
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResponse<StudentDto>
        {
            Items = students.Select(student => student.ToDto(PercentageFor(student))).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<StudentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.AttendanceRecords)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return student?.ToDto(PercentageFor(student));
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureClassExists(dto.ClassId, cancellationToken);
        await EnsureUniqueEnrollmentAndEmail(dto.EnrollmentNumber, dto.Email, null, cancellationToken);

        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLower(),
            EnrollmentNumber = dto.EnrollmentNumber.Trim().ToUpperInvariant(),
            ClassId = dto.ClassId,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(student.Id, cancellationToken))!;
    }

    public async Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw AppValidationException.Single(nameof(id), "Student was not found.");

        await EnsureClassExists(dto.ClassId, cancellationToken);
        await EnsureUniqueEnrollmentAndEmail(dto.EnrollmentNumber, dto.Email, id, cancellationToken);

        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.Email = dto.Email.Trim().ToLower();
        student.EnrollmentNumber = dto.EnrollmentNumber.Trim().ToUpperInvariant();
        student.ClassId = dto.ClassId;
        student.IsActive = dto.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw AppValidationException.Single(nameof(id), "Student was not found.");

        student.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AttendanceHistoryDto> GetHistoryAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students
            .AsNoTracking()
            .Include(s => s.AttendanceRecords)
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken)
            ?? throw AppValidationException.Single(nameof(studentId), "Student was not found.");

        var records = student.AttendanceRecords
            .OrderByDescending(r => r.AttendanceDate)
            .Select(r => r.ToDto())
            .ToList();

        return new AttendanceHistoryDto
        {
            StudentId = student.Id,
            StudentName = $"{student.FirstName} {student.LastName}",
            AttendancePercentage = PercentageFor(student),
            Records = records
        };
    }

    private static decimal PercentageFor(Student student)
    {
        var present = student.AttendanceRecords.Count(r => r.Status == Domain.Enums.AttendanceStatus.Present);
        var late = student.AttendanceRecords.Count(r => r.Status == Domain.Enums.AttendanceStatus.Late);
        var absent = student.AttendanceRecords.Count(r => r.Status == Domain.Enums.AttendanceStatus.Absent);
        var excused = student.AttendanceRecords.Count(r => r.Status == Domain.Enums.AttendanceStatus.Excused);
        return AttendanceCalculator.CalculatePercentage(present, late, absent, excused);
    }

    private async Task EnsureClassExists(Guid classId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Classes.AnyAsync(c => c.Id == classId, cancellationToken);
        if (!exists)
        {
            throw AppValidationException.Single(nameof(classId), "Class was not found.");
        }
    }

    private async Task EnsureUniqueEnrollmentAndEmail(string enrollment, string email, Guid? excludeId, CancellationToken cancellationToken)
    {
        var enrollmentTaken = await dbContext.Students.AnyAsync(
            s => s.EnrollmentNumber == enrollment.Trim().ToUpperInvariant() && s.Id != excludeId,
            cancellationToken);
        if (enrollmentTaken)
        {
            throw AppValidationException.Single(nameof(enrollment), "Enrollment number already exists.");
        }

        var emailTaken = await dbContext.Students.AnyAsync(
            s => s.Email == email.Trim().ToLower() && s.Id != excludeId,
            cancellationToken);
        if (emailTaken)
        {
            throw AppValidationException.Single(nameof(email), "Email already exists.");
        }
    }
}
