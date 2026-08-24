using Atrio.API.Data;
using Atrio.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Atrio.API.Services;

public class StudentService(ApplicationDbContext dbContext) : IStudentService
{
    public async Task<IReadOnlyList<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Students
            .AsNoTracking()
            .Include(student => student.Class)
            .OrderBy(student => student.LastName)
            .Select(student => new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                EnrollmentNumber = student.EnrollmentNumber,
                ClassId = student.ClassId,
                ClassName = student.Class.Name,
                IsActive = student.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
