using Api.Domain.Interfaces;

namespace Api.Domain.Entities;

#pragma warning disable CS8618

public class Budget : IEntity<int>, IHasUser<string>
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public string UserId { get; set; }
    public virtual ICollection<SpendingLimit> SpendingLimits { get; set; }
}
