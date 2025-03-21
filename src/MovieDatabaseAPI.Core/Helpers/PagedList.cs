namespace MovieDatabaseAPI.Core.Helpers;

public class PagedList<T>(List<T> items, int count, int pageNumber, int pageSize)
{
    public List<T> Items { get; } = items;
    public int TotalCount { get; } = count;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}