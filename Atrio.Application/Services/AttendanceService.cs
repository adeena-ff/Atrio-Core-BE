using Atrio.Application.Abstractions;
using Atrio.Application.Common;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Application.Mapping;
using Atrio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Services;

public class AttendanceService(IApplicationDbContext dbContext) : IAttendanceService
{
    public async Task<IReadOnlyList<AttendanceRecordDto>> GetAllAsync(Guid? teacherId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.AttendanceRecords
            .AsNoTracking()
            .Include(r => r.Student)
            .AsQueryable();
        if (teacherId.HasValue) query = query.Where(r => r.Class.TeacherId == teacherId.Value);
        var records = await query.OrderByDescending(r => r.AttendanceDate).ToListAsync(cancellationToken);

        return records.Select(r => r.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<AttendanceRecordDto>> GetByClassAndDateAsync(
        Guid classId,
        DateOnly date,
        Guid? teacherId = null,
        CancellationToken cancellationToken = default)
    {
        if (teacherId.HasValue && !await dbContext.Classes.AnyAsync(c => c.Id == classId && c.TeacherId == teacherId.Value, cancellationToken))
            return [];
        var students = await dbContext.Students
            .AsNoTracking()
            .Where(s => s.ClassId == classId && s.IsActive)
            .OrderBy(s => s.LastName)
            .ToListAsync(cancellationToken);

        var existing = await dbContext.AttendanceRecords
            .AsNoTracking()
            .Include(r => r.Student)
            .Where(r => r.ClassId == classId && r.AttendanceDate == date)
            .ToListAsync(cancellationToken);

        var byStudent = existing.ToDictionary(r => r.StudentId);

        return students.Select(student =>
        {
            if (byStudent.TryGetValue(student.Id, out var record))
            {
                return record.ToDto();
            }

            return new AttendanceRecordDto
            {
                Id = Guid.Empty,
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                ClassId = classId,
                AttendanceDate = date,
                Status = Domain.Enums.AttendanceStatus.Present
            };
        }).ToList();
    }

    public async Task<AttendanceRecordDto> UpsertAsync(UpsertAttendanceDto dto, CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == dto.StudentId, cancellationToken)
            ?? throw AppValidationException.Single(nameof(dto.StudentId), "Student was not found.");

        if (student.ClassId != dto.ClassId)
        {
            throw AppValidationException.Single(nameof(dto.ClassId), "Student is not assigned to this class.");
        }

        var recorderId = dto.RecordedByUserId
            ?? await dbContext.Users.Select(u => (Guid?)u.Id).FirstOrDefaultAsync(cancellationToken)
            ?? throw AppValidationException.Single(nameof(dto.RecordedByUserId), "No recording user is available.");

        var record = await dbContext.AttendanceRecords
            .Include(r => r.Student)
            .FirstOrDefaultAsync(
                r => r.StudentId == dto.StudentId && r.AttendanceDate == dto.AttendanceDate,
                cancellationToken);

        if (record is null)
        {
            record = new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                StudentId = dto.StudentId,
                ClassId = dto.ClassId,
                RecordedByUserId = recorderId,
                AttendanceDate = dto.AttendanceDate,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedAtUtc = DateTime.UtcNow
            };
            dbContext.AttendanceRecords.Add(record);
        }
        else
        {
            record.Status = dto.Status;
            record.Notes = dto.Notes;
            record.ClassId = dto.ClassId;
            record.RecordedByUserId = recorderId;
            record.UpdatedAtUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        record.Student ??= student;
        return record.ToDto();
    }
}
