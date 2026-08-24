using Atrio.API.DTOs;

namespace Atrio.API.Services;

public interface IStudentService
{
    Task<IReadOnlyList<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
