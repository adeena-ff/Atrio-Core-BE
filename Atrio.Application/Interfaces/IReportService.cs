using Atrio.Application.DTOs;

namespace Atrio.Application.Interfaces;

public interface IReportService
{
    Task<MonthlyReportDto> GetMonthlyAsync(MonthlyReportQuery query, CancellationToken cancellationToken = default);
}
