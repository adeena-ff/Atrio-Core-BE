using Atrio.Application.DTOs;

namespace Atrio.Application.Interfaces;

public interface IClassService
{
    Task<IReadOnlyList<ClassDto>> GetAllAsync(Guid? teacherId = null, CancellationToken cancellationToken = default);
    Task<ClassDto> CreateAsync(CreateClassDto dto, CancellationToken cancellationToken = default);
    Task<ClassDto> UpdateAsync(Guid id, UpdateClassDto dto, CancellationToken cancellationToken = default);
    Task AssignStudentAsync(AssignStudentDto dto, CancellationToken cancellationToken = default);
}
