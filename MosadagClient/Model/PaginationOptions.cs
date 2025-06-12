using System.Collections.Generic;

public class PaginationOptions
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
}