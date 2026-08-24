using Atrio.API.DTOs;

namespace Atrio.API.Services;

public interface IClassService
{
    Task<IReadOnlyList<ClassDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
