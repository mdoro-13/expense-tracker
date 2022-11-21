namespace Api.Domain.Entities;

#pragma warning disable CS8618
public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Details { get; set; }
    public string UserId { get; set; }
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }
}
