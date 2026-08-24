using Atrio.Application.DTOs;

namespace Atrio.Application.Interfaces;

public interface IAttendanceService
{
    Task<IReadOnlyList<AttendanceRecordDto>> GetByClassAndDateAsync(Guid classId, DateOnly date, CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto> UpsertAsync(UpsertAttendanceDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecordDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
