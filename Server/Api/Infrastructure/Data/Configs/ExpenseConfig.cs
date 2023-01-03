using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configs;

public class ExpenseConfig : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder
            .Property(e => e.Date)
            .IsRequired();

        builder
            .Property(e => e.Amount)
            .HasConversion<double>()
            .IsRequired();

        builder
            .Property(e => e.UserId)
            .IsRequired();

        builder
            .Property(e => e.CategoryId)
            .IsRequired(false);

        builder
            .Property(e => e.Details)
            .HasMaxLength(200);
    }
}
