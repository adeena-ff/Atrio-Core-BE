using Atrio.Application.Abstractions;
using Atrio.Application.Common;
using Atrio.Application.Common.Models;
using Atrio.Application.Common.Querying;
using Atrio.Application.DTOs;
using Atrio.Application.Interfaces;
using Atrio.Domain.Entities;
using Atrio.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atrio.Application.Services;

public class TeacherService(IApplicationDbContext dbContext) : ITeacherService
{
    public async Task<PagedResponse<TeacherDto>> GetAllAsync(TeacherSearchQuery query, CancellationToken cancellationToken = default)
    {
        var teachersQuery = dbContext.Users
            .Include(user => user.AssignedClasses)
            .Where(user => user.Role == UserRole.Teacher);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            teachersQuery = teachersQuery.Where(user => user.FullName.ToLower().Contains(term) || user.Email.ToLower().Contains(term));
        }

        if (query.ClassId.HasValue) teachersQuery = teachersQuery.Where(user => user.AssignedClasses.Any(c => c.Id == query.ClassId.Value));

        var departmentCode = QueryFilter.DepartmentCode(query.Department);
        if (departmentCode is not null) teachersQuery = teachersQuery.Where(user => user.AssignedClasses.Any(c => c.Code.StartsWith(departmentCode + "-")));

        var (pageNumber, pageSize) = QueryFilter.NormalizePage(query.PageNumber, query.PageSize);
        var totalCount = await teachersQuery.CountAsync(cancellationToken);
        var teachers = await teachersQuery.OrderBy(user => user.FullName).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync(cancellationToken);

        return new PagedResponse<TeacherDto>
        {
            Items = teachers.Select(ToDto).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TeacherDto> CreateAsync(CreateTeacherDto dto, CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken))
            throw AppValidationException.Single(nameof(dto.Email), "Email address is already in use.");
        var classes = await GetAssignedClassesAsync(dto.AssignedClassIds, cancellationToken);
        var teacher = new User { Id = Guid.NewGuid(), FullName = dto.FullName.Trim(), Email = email, Role = UserRole.Teacher, IsActive = true, CreatedAtUtc = DateTime.UtcNow };
        teacher.PasswordHash = new PasswordHasher<User>().HashPassword(teacher, dto.Password);
        foreach (var classEntity in classes) classEntity.TeacherId = teacher.Id;
        dbContext.Users.Add(teacher);
        await dbContext.SaveChangesAsync(cancellationToken);
        teacher.AssignedClasses = classes;
        return ToDto(teacher);
    }

    public async Task<TeacherDto> UpdateAsync(Guid id, UpdateTeacherDto dto, CancellationToken cancellationToken = default)
    {
        var teacher = await dbContext.Users.Include(user => user.AssignedClasses)
            .FirstOrDefaultAsync(user => user.Id == id && user.Role == UserRole.Teacher, cancellationToken)
            ?? throw AppValidationException.Single(nameof(id), "Teacher was not found.");
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(user => user.Email == email && user.Id != id, cancellationToken))
            throw AppValidationException.Single(nameof(dto.Email), "Email address is already in use.");
        var classes = await GetAssignedClassesAsync(dto.AssignedClassIds, cancellationToken);
        foreach (var classEntity in teacher.AssignedClasses) classEntity.TeacherId = null;
        foreach (var classEntity in classes) classEntity.TeacherId = teacher.Id;
        teacher.FullName = dto.FullName.Trim(); teacher.Email = email; teacher.IsActive = dto.IsActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        teacher.AssignedClasses = classes;
        return ToDto(teacher);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id && user.Role == UserRole.Teacher, cancellationToken)
            ?? throw AppValidationException.Single(nameof(id), "Teacher was not found.");
        teacher.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<Class>> GetAssignedClassesAsync(IReadOnlyList<Guid> assignedClassIds, CancellationToken cancellationToken)
    {
        var classIds = assignedClassIds.Distinct().ToArray();
        var classes = await dbContext.Classes.Where(classEntity => classIds.Contains(classEntity.Id)).ToListAsync(cancellationToken);
        if (classes.Count != classIds.Length) throw AppValidationException.Single(nameof(assignedClassIds), "One or more assigned classes were not found.");
        return classes;
    }

    private static TeacherDto ToDto(User user) => new() { Id = user.Id, FullName = user.FullName, Email = user.Email, IsActive = user.IsActive, AssignedClassIds = user.AssignedClasses.Select(classEntity => classEntity.Id).ToList() };
}
