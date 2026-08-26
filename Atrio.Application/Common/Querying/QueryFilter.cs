namespace Atrio.Application.Common.Querying;

/// <summary>Normalizes public list-query values before they are applied to EF Core queries.</summary>
public static class QueryFilter
{
    public const int DefaultPageSize = 10;
    public const int MaximumPageSize = 100;

    public static (int PageNumber, int PageSize) NormalizePage(int pageNumber, int pageSize) =>
        (Math.Max(1, pageNumber), Math.Clamp(pageSize, 1, MaximumPageSize));

    public static string? DepartmentCode(string? department)
    {
        if (string.IsNullOrWhiteSpace(department)) return null;

        return department.Trim().ToUpperInvariant() switch
        {
            "CSC" or "COMPUTER SCIENCE" => "CSC",
            "MTH" or "MATHEMATICS" => "MTH",
            "PHY" or "PHYSICS" => "PHY",
            "HIS" or "HUMANITIES" or "HISTORY" => "HIS",
            var value when value.Length is 3 => value,
            _ => null
        };
    }
}
