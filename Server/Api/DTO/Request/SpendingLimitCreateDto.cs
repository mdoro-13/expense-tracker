namespace Api.DTO.Request;
#pragma warning disable CS8618

public sealed class SpendingLimitCreateDto
{
    public decimal Amount { get; init; }
    public int? CategoryId { get; init; }
    public int BudgetId { get; init; }
}
