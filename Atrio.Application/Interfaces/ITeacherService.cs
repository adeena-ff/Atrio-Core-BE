using Atrio.Application.DTOs;

namespace Atrio.Application.Interfaces;

public interface ITeacherService
{
    Task<IReadOnlyList<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TeacherDto> CreateAsync(CreateTeacherDto dto, CancellationToken cancellationToken = default);
    Task<TeacherDto> UpdateAsync(Guid id, UpdateTeacherDto dto, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
