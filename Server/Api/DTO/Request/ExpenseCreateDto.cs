namespace Api.DTO.Request;
#pragma warning disable CS8618

public sealed record ExpenseCreateDto
{
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Details { get; init; }
    public int? CategoryId { get; init; }
}
