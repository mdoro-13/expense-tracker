using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configs;

public class BudgetConfig : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder
            .Property(b => b.StartDate)
            .IsRequired();

        builder
            .Property(b => b.EndDate)
            .IsRequired();

        builder
            .Property(b => b.Amount)
            .IsRequired();

        builder
            .Property(b => b.UserId)
            .IsRequired();

        builder
            .HasMany(b => b.SpendingLimits)
            .WithOne(sp => sp.Budget)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
