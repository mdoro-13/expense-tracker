namespace Api.DTO.Response;
#pragma warning disable CS8618

public sealed record ExpenseReadDto
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Details { get; init; }
    public int? CategoryId { get; init; }
}
