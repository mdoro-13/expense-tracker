namespace Api.DTO.Response;
#pragma warning disable CS8618

public sealed record BudgetDetailsDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalSpent { get; set; }
    public ICollection<CategorySpendingLimit> CategorySpendingLimits { get; set; }
}

public sealed record CategorySpendingLimit
{
    public string Name { get; set; }
    public decimal Limit { get; set; }
    public decimal Spent { get; set; }
}
