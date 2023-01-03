using Api.Domain.Interfaces;

namespace Api.Domain.Entities;

#pragma warning disable CS8618

public class SpendingLimit : IEntity<int>
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int BudgetId { get; set; }
    public Budget Budget { get; set; }

}
