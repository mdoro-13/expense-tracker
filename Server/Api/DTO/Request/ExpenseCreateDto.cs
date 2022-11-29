namespace Api.DTO.Request;
#pragma warning disable CS8618

public sealed record ExpenseCreateDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Details { get; set; }
    public int? CategoryId { get; set; }
}
