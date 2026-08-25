using Atrio.Application.DTOs;

namespace Atrio.Application.Interfaces;

public interface IStudentService
{
    Task<IReadOnlyList<StudentDto>> SearchAsync(StudentSearchQuery query, CancellationToken cancellationToken = default);
    Task<StudentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<AttendanceHistoryDto> GetHistoryAsync(Guid studentId, CancellationToken cancellationToken = default);
}
