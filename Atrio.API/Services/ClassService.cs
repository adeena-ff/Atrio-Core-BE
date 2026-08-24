using Atrio.API.Data;
using Atrio.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Atrio.API.Services;

public class ClassService(ApplicationDbContext dbContext) : IClassService
{
    public async Task<IReadOnlyList<ClassDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Classes
            .AsNoTracking()
            .OrderBy(classEntity => classEntity.Name)
            .Select(classEntity => new ClassDto
            {
                Id = classEntity.Id,
                Name = classEntity.Name,
                Code = classEntity.Code,
                AcademicYear = classEntity.AcademicYear,
                IsActive = classEntity.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
