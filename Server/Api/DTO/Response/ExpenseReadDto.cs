namespace Api.DTO.Response;
#pragma warning disable CS8618

public sealed record ExpenseReadDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Details { get; set; }
    public int? CategoryId { get; set; }
}
