using System.Security.Claims;
using Api.Controllers;
using Api.Domain.Entities;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Query;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using UnitTests.Utils;
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
    public async Task GetAll_Sort_And_Filter()
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
            await SortByDate_And_FilterByDate(dbContext, expense3.Date, expense1.Date, expense4);
            await SortByAmount_And_FilterByDateAndCategory(dbContext, expense1.Amount, expense2.Amount);
        }
    }

    [Theory]
    [InlineData(1, 10, 50)]
    [InlineData(1, 101, 110)]
    [InlineData(1, 80, 50)]
    [InlineData(2, 50, 40)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 10, 20)]
    [InlineData(2, 5, 25)]
    [InlineData(3, 7, 28)]
    public async Task GetAll_Pagination(int pageNumber, int pageSize, int generatedExpenses)
    {
        var random = new Random();
        const int maxPerPage = 100;
        using (var connection = new SqliteConnection("DataSource=:memory:"))
        {
            var dbContext = await InitializeDb(connection);
            await dbContext.Set<Category>().AddRangeAsync(_defaultCategories);
            await dbContext.SaveChangesAsync();

            for (int i = 0; i < generatedExpenses; i++)
            {
                var expense = new Expense
                {
                    CategoryId = random.Next(1, 2),
                    Amount = random.Next(1, 5000),
                    UserId = UserId,
                    Date = new DateTime(2022, DateTime.UtcNow.Month, random.Next(1, 28)),
                };
                await dbContext.Set<Expense>().AddAsync(expense);
            }

            await dbContext.SaveChangesAsync();

            var controller = ControllerTestUtils.InitializeController<ExpenseController>(dbContext, _user);
            var paging = new ExpensePaging
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await controller.Get(new ExpenseFilter(), new ExpenseSort(), paging);
            result.Should().BeOfType<OkObjectResult>();

            //var header = controller.Response.Headers["Pagination"].

            var resultValue = ResultCollectionValue<ExpenseReadDto>(result);

            if (pageSize > maxPerPage && pageSize <= generatedExpenses && pageNumber == 1)
            {
                resultValue.Count.Should().Be(maxPerPage);
            }
            else if (pageSize <= generatedExpenses && pageNumber == 1)
            {
                resultValue.Count.Should().Be(pageSize);
            }
            else if (pageSize >= generatedExpenses && pageNumber == 1)
            {
                resultValue.Count.Should().Be(generatedExpenses);
            }

            if (pageNumber > 1 && pageSize >= generatedExpenses)
            {
                resultValue.Count.Should().Be(0);
            }

        }

    }

    [Fact]
    public async Task GetById_Returns_200_If_Found()
    {
        using (var connection = new SqliteConnection("DataSource=:memory:"))
        {
            var dbContext = await InitializeDb(connection);
            await dbContext.Set<Category>().AddRangeAsync(_defaultCategories);

            var expense = new Expense
            {
                CategoryId = 1,
                Amount = 200M,
                Date = DateTime.UtcNow,
                UserId = UserId,
                Details = "test"
            };

            await dbContext.Set<Expense>().AddAsync(expense);
            await dbContext.SaveChangesAsync();

            var controller = ControllerTestUtils.InitializeController<ExpenseController>(dbContext, _user);
            var result = await controller.Get(1);
            result.Should().BeOfType<OkObjectResult>();

        }
    }

    [Fact]
    public async Task GetById_Returns_404_If_Not_Found()
    {
        using (var connection = new SqliteConnection("DataSource=:memory:"))
        {
            var dbContext = await InitializeDb(connection);
            await dbContext.Set<Category>().AddRangeAsync(_defaultCategories);

            var expense = new Expense
            {
                CategoryId = 1,
                Amount = 200M,
                Date = DateTime.UtcNow,
                UserId = UserId,
                Details = "test"
            };

            await dbContext.Set<Expense>().AddAsync(expense);
            await dbContext.SaveChangesAsync();

            var controller = ControllerTestUtils.InitializeController<ExpenseController>(dbContext, _user);
            var result = await controller.Get(2);
            result.Should().BeOfType<NotFoundResult>();

        }
    }

    [Fact]
    public async Task GetById_Returns_404_If_Does_Not_Belong_To_User()
    {
        using (var connection = new SqliteConnection("DataSource=:memory:"))
        {
            var dbContext = await InitializeDb(connection);
            await dbContext.Set<Category>().AddRangeAsync(_defaultCategories);

            var expense = new Expense
            {
                CategoryId = 1,
                Amount = 200M,
                Date = DateTime.UtcNow,
                UserId = "otherUser",
                Details = "test"
            };

            await dbContext.Set<Expense>().AddAsync(expense);
            await dbContext.SaveChangesAsync();

            var controller = ControllerTestUtils.InitializeController<ExpenseController>(dbContext, _user);
            var result = await controller.Get(1);
            result.Should().BeOfType<NotFoundResult>();

        }
    }

    private async Task SortByAmount_And_FilterByDateAndCategory(DataContext dbContext, decimal highestAmount, decimal lowestAmount)
    {
        var controller = ControllerTestUtils.InitializeController<ExpenseController>(dbContext, _user);

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

        var result = await controller.Get(filterByDateAndCategory, sortByAmount, firstPage25PageSize);
        result.Should().BeOfType<OkObjectResult>();

        var resultValue = ResultCollectionValue<ExpenseReadDto>(result);

        var educationCategoryExcluded = resultValue.All(e => e.CategoryId != 2);
        var highestAmountFirst = resultValue.FirstOrDefault().Amount == highestAmount;
        var lowestAmountLast = resultValue.Last().Amount == lowestAmount;
        
        educationCategoryExcluded.Should().BeTrue();
        highestAmountFirst.Should().BeTrue();
        lowestAmountLast.Should().BeTrue();
    }
    
    private async Task SortByDate_And_FilterByDate(DataContext dbContext, DateTime greatestDate, DateTime lowestDate, Expense outOfTimeRangeExpense)
    {
        var controller = ControllerTestUtils.InitializeController<ExpenseController>(dbContext, _user);

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

        var resultValue = ResultCollectionValue<ExpenseReadDto>(result);
        
        var greatestDateFirst = resultValue
            .FirstOrDefault()
            .Date == greatestDate;

        var lowestDateLast = resultValue
            .Last()
            .Date == lowestDate;

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
    private ICollection<T> ResultCollectionValue<T>(IActionResult result)
    {
        return (result as OkObjectResult)
            .Value
            .As<ICollection<T>>();
    }
}