using Atrio.Domain.Enums;

namespace Atrio.Domain.Services;

public static class AttendanceCalculator
{
    /// <summary>
    /// Present and Late count as attended. Excused is excluded from the denominator.
    /// </summary>
    public static decimal CalculatePercentage(int present, int late, int absent, int excused)
    {
        _ = excused;
        var eligible = present + late + absent;
        if (eligible == 0)
        {
            return 0m;
        }

        return Math.Round((present + late) * 100m / eligible, 2);
    }

    public static bool CountsAsPresent(AttendanceStatus status) =>
        status is AttendanceStatus.Present or AttendanceStatus.Late;

    public static bool CountsAsEligibleSession(AttendanceStatus status) =>
        status is not AttendanceStatus.Excused;
}
