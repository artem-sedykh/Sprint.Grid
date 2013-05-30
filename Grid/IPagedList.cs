namespace Sprint.Grid
{
    public interface IPagedList
    {        
        int PageCount { get; }
        
        int TotalItemCount { get; }        
        
        int PageNumber { get; }
        
        int PageSize { get; }
        
        bool HasPreviousPage { get; }
        
        bool HasNextPage { get; }
        
        bool IsFirstPage { get; }
        
        bool IsLastPage { get; }
        
        int FirstItemOnPage { get; }
        
        int LastItemOnPage { get; }
    }
}
