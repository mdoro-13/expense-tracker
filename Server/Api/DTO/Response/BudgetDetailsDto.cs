namespace Api.DTO.Response;
#pragma warning disable CS8618

public sealed record BudgetDetailsDto
{
    public int Id { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal Amount { get; init; }
    public decimal TotalSpent { get; init; }
    public ICollection<CategorySpendingLimit> CategorySpendingLimits { get; init; }
}

public sealed record CategorySpendingLimit
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Limit { get; set; }
    public decimal Spent { get; set; }
}
