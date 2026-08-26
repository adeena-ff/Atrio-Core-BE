namespace Atrio.Application.Common.Querying;

/// <summary>Normalizes public list-query values before they are applied to EF Core queries.</summary>
public static class QueryFilter
{
    public const int DefaultPageSize = 10;
    public const int MaximumPageSize = 100;

    public static (int PageNumber, int PageSize) NormalizePage(int pageNumber, int pageSize) =>
        (Math.Max(1, pageNumber), Math.Clamp(pageSize, 1, MaximumPageSize));

}
