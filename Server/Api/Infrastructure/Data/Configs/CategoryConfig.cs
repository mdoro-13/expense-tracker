using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configs;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(c => c.Color)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(6);

        builder
            .HasMany(c => c.Expenses)
            .WithOne(e => e.Category)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(c => c.SpendingLimits)
            .WithOne(sp => sp.Category)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
