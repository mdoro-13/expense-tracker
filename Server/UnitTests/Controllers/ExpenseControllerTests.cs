using System.Security.Claims;
using Api.Controllers;
using Api.Domain.Entities;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Query;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.Controllers;

public sealed class ExpenseControllerTests
{
    private const string UserId = "test-id";
    private const string DefaultColorValue = "xxxxxx";
    private readonly ClaimsPrincipal _user;
    private readonly List<Category> _defaultCategories = new List<Category>();

    public ExpenseControllerTests()
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
    public async Task GetAll()
    {
        using (var connection = new SqliteConnection("DataSource=:memory:"))
        {
            // ARRANGE
            var dbContext = await InitializeDb(connection);

            await dbContext.Set<Category>().AddRangeAsync(_defaultCategories);
            await dbContext.SaveChangesAsync();

            var expense1 = new Expense
            {
                Amount = 300,
                UserId = UserId,
                Details = "Potatoes",
                CategoryId = 1,
                Date = new DateTime(2022,1,1)
            };            
            var expense2 = new Expense
            {
                Amount = 250,
                UserId = UserId,
                Details = "Beef",
                CategoryId = 1,
                Date = new DateTime(2022,1,2)
            };
            var expense3 = new Expense
            {
                Amount = 825.55M,
                UserId = UserId,
                Details = "Pluralsight subscription",
                CategoryId = 2,
                Date = new DateTime(2022,2,5)
            };
            
            var expense4 = new Expense
            {
                Amount = 5000.25M,
                UserId = UserId,
                Details = "Book",
                CategoryId = 2,
                Date = new DateTime(2022,6,1)
            };

            await dbContext.Set<Expense>().AddAsync(expense1);
            await dbContext.Set<Expense>().AddAsync(expense2);
            await dbContext.Set<Expense>().AddAsync(expense3);
            await dbContext.Set<Expense>().AddAsync(expense4);
            await dbContext.SaveChangesAsync();
            
            // ACT
            await SortByDate_And_FilterByDate(dbContext, expense3, expense1, expense4);
            await SortByAmount_And_FilterByDateAndCategory(dbContext, expense1.Amount, expense2.Amount);
        }
    }

    private async Task SortByAmount_And_FilterByDateAndCategory(DataContext dbContext, decimal highestAmount, decimal lowestAmount)
    {
        var filterByDateAndCategory = new ExpenseFilter
        {
            DateFrom = new DateTime(2022, 1, 1),
            DateTo = new DateTime(2022, 6, 1),
            CategoryIds = new List<int?>() { 1 }
        };

        var sortByAmount = new ExpenseSort
        {
            SortBy = "amount",
            Direction = Direction.Descending
        };

        var firstPage25PageSize = new ExpensePaging
        {
            PageNumber = 1,
            PageSize = 25
        };

        var controller = new ExpenseController(dbContext);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = _user
            }
        };

        var result = await controller.Get(filterByDateAndCategory, sortByAmount, firstPage25PageSize);
        result.Should().BeOfType<OkObjectResult>();

        var resultValue = ResultValue<ExpenseReadDto>(result);

        var educationCategoryExcluded = resultValue.All(e => e.CategoryId != 2);
        var highestAmountFirst = resultValue.FirstOrDefault().Amount == highestAmount;
        var lowestAmountLast = resultValue.Last().Amount == lowestAmount;
        
        educationCategoryExcluded.Should().BeTrue();
        highestAmountFirst.Should().BeTrue();
        lowestAmountLast.Should().BeTrue();
    }
    
    private async Task SortByDate_And_FilterByDate(DataContext dbContext, Expense expense3, Expense expense1, Expense outOfTimeRangeExpense)
    {
        var controller = new ExpenseController(dbContext);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = _user
            }
        };

        var filterByDate = new ExpenseFilter
        {
            DateFrom = new DateTime(2022, 1, 1),
            DateTo = new DateTime(2022, 5, 1)
        };
        var sortByDate = new ExpenseSort
        {
            SortBy = "date",
            Direction = Direction.Descending
        };
        var firstPage25PageSize = new ExpensePaging
        {
            PageNumber = 1,
            PageSize = 25
        };

        var result = await controller.Get(filterByDate, sortByDate, firstPage25PageSize);
        result.Should().BeOfType<OkObjectResult>();

        var resultValue = ResultValue<ExpenseReadDto>(result);
        
        var greatestDateFirst = resultValue
            .FirstOrDefault()
            .Date == expense3.Date;

        var lowestDateLast = resultValue
            .Last()
            .Date == expense1.Date;

        var outOfTimeRangeExpenseIsExcluded = resultValue.All(e => e.Date != outOfTimeRangeExpense.Date);

        // ASSERT
        greatestDateFirst.Should().BeTrue();
        lowestDateLast.Should().BeTrue();
        outOfTimeRangeExpenseIsExcluded.Should().BeTrue();
    }

    private async Task<DataContext> InitializeDb(SqliteConnection connection)
    {
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new DataContext(options);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
    private ICollection<T> ResultValue<T>(IActionResult result)
    {
        return (result as OkObjectResult)
            .Value
            .As<ICollection<T>>();
    }
}