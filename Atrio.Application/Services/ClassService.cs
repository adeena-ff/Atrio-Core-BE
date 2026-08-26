using Atrio.Application.Abstractions;
using Atrio.Application.Common;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Application.Mapping;
using Atrio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Services;

public class ClassService(IApplicationDbContext dbContext) : IClassService
{
    public async Task<IReadOnlyList<ClassDto>> GetAllAsync(Guid? teacherId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Classes
            .AsNoTracking()
            .Include(c => c.Students)
            .AsQueryable();
        if (teacherId.HasValue) query = query.Where(c => c.TeacherId == teacherId.Value);
        var classes = await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);

        return classes.Select(c => c.ToDto(c.Students.Count(s => s.IsActive))).ToList();
    }

    public async Task<ClassDto> CreateAsync(CreateClassDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureUniqueCode(dto.Code, null, cancellationToken);

        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Code = dto.Code.Trim().ToUpperInvariant(),
            AcademicYear = dto.AcademicYear.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Classes.Add(classEntity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return classEntity.ToDto();
    }

    public async Task<ClassDto> UpdateAsync(Guid id, UpdateClassDto dto, CancellationToken cancellationToken = default)
    {
        var classEntity = await dbContext.Classes
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw AppValidationException.Single(nameof(id), "Class was not found.");

        await EnsureUniqueCode(dto.Code, id, cancellationToken);

        classEntity.Name = dto.Name.Trim();
        classEntity.Code = dto.Code.Trim().ToUpperInvariant();
        classEntity.AcademicYear = dto.AcademicYear.Trim();
        classEntity.IsActive = dto.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return classEntity.ToDto(classEntity.Students.Count(s => s.IsActive));
    }

    public async Task AssignStudentAsync(AssignStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == dto.StudentId, cancellationToken)
            ?? throw AppValidationException.Single(nameof(dto.StudentId), "Student was not found.");

        var classExists = await dbContext.Classes.AnyAsync(c => c.Id == dto.ClassId, cancellationToken);
        if (!classExists)
        {
            throw AppValidationException.Single(nameof(dto.ClassId), "Class was not found.");
        }

        student.ClassId = dto.ClassId;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueCode(string code, Guid? excludeId, CancellationToken cancellationToken)
    {
        var taken = await dbContext.Classes.AnyAsync(
            c => c.Code == code.Trim().ToUpperInvariant() && c.Id != excludeId,
            cancellationToken);
        if (taken)
        {
            throw AppValidationException.Single(nameof(code), "Class code already exists.");
        }
    }
}
