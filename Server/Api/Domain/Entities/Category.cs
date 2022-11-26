namespace Api.Domain.Entities;

#pragma warning disable CS8618

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string UserId { get; set; }
    public virtual ICollection<Expense> Expenses { get; set; }
    public virtual ICollection<SpendingLimit> SpendingLimits { get; set; }
}
