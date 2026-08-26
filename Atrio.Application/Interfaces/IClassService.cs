using Atrio.Application.Common.Models;
using Atrio.Application.DTOs;

namespace Atrio.Application.Interfaces;

public interface IClassService
{
    Task<PagedResponse<ClassDto>> GetAllAsync(ClassSearchQuery query, CancellationToken cancellationToken = default);
    Task<ClassDto> CreateAsync(CreateClassDto dto, CancellationToken cancellationToken = default);
    Task<ClassDto> UpdateAsync(Guid id, UpdateClassDto dto, CancellationToken cancellationToken = default);
    Task AssignStudentAsync(AssignStudentDto dto, CancellationToken cancellationToken = default);
}
