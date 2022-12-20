using Api.Domain.Entities;
using Api.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UnitTests.Utils;
using Xunit;

namespace UnitTests.Services;

public sealed class ExpenseManagerTests
{
    private const string UserId = "test-id";
    private readonly ClaimsPrincipal _user;
    private const string DefaultColorValue = "xxxxxx";


    private readonly List<Category> _defaultCategories = new List<Category>();


    public ExpenseManagerTests()
	{
        _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("user_id", UserId),
        }, "mock"));

        _defaultCategories.Add(new Category
        {
            Name = "Groceries",
            UserId = UserId,
            Color = DefaultColorValue
        });

        _defaultCategories.Add(new Category
        {
            Name = "Education",
            UserId = UserId,
            Color = DefaultColorValue
        });
    }

	[Fact]
	public async Task CalculateBudgetStatsAsync_Returns_Default_If_No_Budget()
	{
        using (var connection = new SqliteConnection("DataSource=:memory:"))
		{
			var dbContext = await DbMockUtils.InitializeDbAsync(connection);
			var sut = new ExpenseManager(dbContext);

			var result = await sut.CalculateBudgetStatsAsync(1, _user);

			result.Should().BeNull();
		}
    }

    [Fact]
    public async Task CalculateBudgetStatsAsync_Returns_Correct_Spent_Amounts()
    {
        using (var connection = new SqliteConnection("DataSource=:memory:"))
        {
            var dbContext = await DbMockUtils.InitializeDbAsync(connection);
            await dbContext.Set<Category>().AddRangeAsync(_defaultCategories);
            await dbContext.SaveChangesAsync();

            var expense1 = new Expense
            {
                CategoryId = 1,
                Amount = 299.11M,
                UserId = UserId,
                Date = new DateTime(2022, 1, 1)
            };
            var expense2 = new Expense
            {
                CategoryId = 1,
                Amount = 391.43M,
                UserId = UserId,
                Date = new DateTime(2022, 1, 2)
            };
            var expense3 = new Expense
            {
                CategoryId = 1,
                Amount = 1112.49M,
                UserId = UserId,
                Date = new DateTime(2022, 1, 31)
            };
            var expense4 = new Expense
            {
                CategoryId = 2,
                Amount = 767.48M,
                UserId = UserId,
                Date = new DateTime(2022, 1, 30)
            };

            await dbContext.Set<Expense>().AddAsync(expense1);
            await dbContext.Set<Expense>().AddAsync(expense2);
            await dbContext.Set<Expense>().AddAsync(expense3);
            await dbContext.Set<Expense>().AddAsync(expense4);
            await dbContext.SaveChangesAsync();

            var budget = new Budget
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 31),
                Amount = 5000M,
                UserId = UserId
            };
            await dbContext.Set<Budget>().AddAsync(budget);
            await dbContext.SaveChangesAsync();

            var spendingLimit1 = new SpendingLimit
            {
                CategoryId = 1,
                Amount = 999.99M,
                BudgetId = 1,

            };
            var spendingLimit2 = new SpendingLimit
            {
                CategoryId = 2,
                Amount = 999.99M,
                BudgetId = 1,

            };
            await dbContext.Set<SpendingLimit>().AddAsync(spendingLimit1);
            await dbContext.Set<SpendingLimit>().AddAsync(spendingLimit2);
            await dbContext.SaveChangesAsync();

            var sut = new ExpenseManager(dbContext);

            var result = await sut.CalculateBudgetStatsAsync(1, _user);
            var expectedSpent = 2570.51M;
            var category1TotalSpent = 1803.03M;
            var category2TotalSpent = 767.48M;

            result!.TotalSpent.Should().Be(expectedSpent);
            result!.CategorySpendingLimits.ToArray()[0].Spent.Should().Be(category1TotalSpent);
            result!.CategorySpendingLimits.ToArray()[1].Spent.Should().Be(category2TotalSpent);
        }
    }

    [Fact]
    public async Task BudgetExistsForExpenseDateAsync_Returns_Correct_Result_Based_On_Expense_Date()
    {
        using (var connection = new SqliteConnection("DataSource=:memory:"))
        {
            var dbContext = await DbMockUtils.InitializeDbAsync(connection);
            var budget = new Budget
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 31),
                Amount = 5000M,
                UserId = UserId
            };
            await dbContext.Set<Budget>().AddAsync(budget);
            await dbContext.SaveChangesAsync();

            var sut = new ExpenseManager(dbContext);
            var trueResult1 = await sut.BudgetExistsForExpenseDateAsync(new DateTime(2022, 1, 1), _user);
            var trueResult2 = await sut.BudgetExistsForExpenseDateAsync(new DateTime(2022, 1, 31), _user);
            var trueResult3 = await sut.BudgetExistsForExpenseDateAsync(new DateTime(2022, 1, 15), _user);

            var falseResult1 = await sut.BudgetExistsForExpenseDateAsync(new DateTime(2021, 12, 31), _user);
            var falseResult2 = await sut.BudgetExistsForExpenseDateAsync(new DateTime(2022, 2, 1), _user);
            var falseResult3 = await sut.BudgetExistsForExpenseDateAsync(new DateTime(2022, 2, 3), _user);

            trueResult1.Should().BeTrue();
            trueResult2.Should().BeTrue();
            trueResult3.Should().BeTrue();

            falseResult1.Should().BeFalse();
            falseResult2.Should().BeFalse();
            falseResult3.Should().BeFalse();

        }

    }

}
