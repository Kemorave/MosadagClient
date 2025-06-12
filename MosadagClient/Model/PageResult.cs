using System;
using System.Collections.Generic;

public class PaginatedResult<T>
{
    public IEnumerable<T>? Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public PaginationOptions? PaginationOptions { get; set; }
}
