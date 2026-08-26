using Atrio.Application.DTOs;

namespace Atrio.Application.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardAnalyticsDto> GetDashboardAsync(Guid? teacherId, CancellationToken cancellationToken = default);
    Task<ReportAnalyticsDto> GetReportsAsync(Guid? teacherId, Guid? classId, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken = default);
}
