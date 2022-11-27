using Api.Domain.Entities;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data.Seed;

public static class CategoriesSeed
{
    private const string USER_ID = "nnee3bNG9EYXRaUkujKLNnXa0qm1";
    private static readonly string[] FAKE_CATEGORIES = { "Groceries", "Going out", "Education", "Car", "Vacations" };
    private static readonly string[] COLOR_CODES = { "2596be", "2297bf", "551b01", "9dbbc3" };
    public static async Task SeedCategoriesAsync(DataContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        foreach (var categoryName in FAKE_CATEGORIES)
        {
            var category = new Faker<Category>()
                .RuleFor(x => x.Name, categoryName)
                .RuleFor(x => x.Color, x => x.PickRandom(COLOR_CODES))
                .RuleFor(x => x.UserId, USER_ID);

            await context.AddAsync(category.Generate());
                
        }

        await context.SaveChangesAsync();
    }
}
