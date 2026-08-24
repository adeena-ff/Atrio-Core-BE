using Atrio.API.Data;
using Atrio.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Atrio.API.Services;

public class AttendanceService(ApplicationDbContext dbContext) : IAttendanceService
{
    public async Task<IReadOnlyList<AttendanceRecordDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AttendanceRecords
            .AsNoTracking()
            .OrderByDescending(record => record.AttendanceDate)
            .Select(record => new AttendanceRecordDto
            {
                Id = record.Id,
                StudentId = record.StudentId,
                ClassId = record.ClassId,
                RecordedByUserId = record.RecordedByUserId,
                AttendanceDate = record.AttendanceDate,
                Status = record.Status,
                Notes = record.Notes
            })
            .ToListAsync(cancellationToken);
    }
}
