using Atrio.API.DTOs;

namespace Atrio.API.Services;

public interface IAttendanceService
{
    Task<IReadOnlyList<AttendanceRecordDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
