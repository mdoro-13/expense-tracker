namespace Api.Utils.Query;

public sealed record ExpensePaging 
{
    private const int MaxPageSize = 100;
    public int PageNumber { get; set; } = 1;
    private int _pageSize { get; set; } = 25;
    public int PageSize 
    { 
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

}

public sealed record ExpenseFilter
{
    public DateTime DateFrom { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime DateTo { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public int?[] CategoryIds { get; set; } = new int?[] {};
}

public sealed record ExpenseSort 
{
    public string SortBy { get; set; } = "date";
    public Direction Direction { get; set; } = Direction.Descending;
}

public enum Direction 
{
    Ascending = 1,
    Descending = -1
}