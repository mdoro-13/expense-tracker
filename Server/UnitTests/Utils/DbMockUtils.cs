using Api.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Utils;

public static class DbMockUtils
{
    public static async Task<DataContext> InitializeDbAsync(SqliteConnection connection)
    {
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new DataContext(options);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}
