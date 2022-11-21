namespace Api.Domain.Entities;

#pragma warning disable CS8618

public class SpendingLimit
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public int BudgetId { get; set; }
    public virtual Budget Budget { get; set; }

}
