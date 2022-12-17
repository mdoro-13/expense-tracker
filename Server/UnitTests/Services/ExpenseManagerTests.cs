using Api.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using System.Security.Claims;
using UnitTests.Utils;
using Xunit;

namespace UnitTests.Services;

public sealed class ExpenseManagerTests
{
    private const string UserId = "test-id";
    private readonly ClaimsPrincipal _user;

    public ExpenseManagerTests()
	{
        _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("user_id", UserId),
        }, "mock"));
    }

	[Fact]
	public async Task CalculateBudgetStatsAsync_Returns_Default_If_No_Budget()
	{
        using (var connection = new SqliteConnection("DataSource=:memory:"))
		{
			var context = await DbMockUtils.InitializeDbAsync(connection);
			var sut = new ExpenseManager(context);

			var result = await sut.CalculateBudgetStatsAsync(1, _user);

			result.Should().BeNull();
		}
    }
}
