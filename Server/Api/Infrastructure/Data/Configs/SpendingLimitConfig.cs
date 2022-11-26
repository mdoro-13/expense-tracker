using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configs;

public class SpendingLimitConfig : IEntityTypeConfiguration<SpendingLimit>
{
    public void Configure(EntityTypeBuilder<SpendingLimit> builder)
    {
        builder
            .Property(sp => sp.Amount)
            .IsRequired();

        builder
            .Property(sp => sp.CategoryId)
            .IsRequired(false);

        builder
            .Property(sp => sp.BudgetId)
            .IsRequired();
    }
}
