namespace ERP.Application.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 10));
    public bool HasNextPage => PageIndex < TotalPages;
    public bool HasPreviousPage => PageIndex > 1;

    public PagedResult() { }

    public PagedResult(List<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}

public class ProblemDetailsDto
{
    public string Title { get; set; } = "Erro na requisição";
    public int Status { get; set; } = 400;
    public string? Detail { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}
