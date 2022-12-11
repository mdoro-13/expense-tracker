using Api.Domain.Entities;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Services
{
    public class ExpenseManager : IExpenseManager
    {
        private readonly DataContext _context;
        public ExpenseManager(DataContext context)
        {
            _context = context;
        }

        public async Task<BudgetDetailsDto?> CalculateBudgetStatsAsync(int id, ClaimsPrincipal user)
        {
            var budget = await _context
                .BelongsToUser<Budget>(user)
                .Include(b => b.SpendingLimits)
                    .ThenInclude(x => x.Category)
                        .ThenInclude(x => x.Expenses)
                .Where(b => b.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (budget is null)
            {
                return default;
            }

            decimal totalSpent = 0;
            ICollection<CategorySpendingLimit> categorySpendingLimits = new List<CategorySpendingLimit>();

            foreach (var sp in budget.SpendingLimits)
            {
                var categorySpendingLimit = new CategorySpendingLimit
                {
                    Name = sp!.Category!.Name,
                    Limit = sp.Amount,
                    Spent = sp.Category.Expenses.Sum(x => x.Amount)
                };
                totalSpent += categorySpendingLimit.Spent;
                categorySpendingLimits.Add(categorySpendingLimit);
            }

            return new BudgetDetailsDto
            {
                Id = budget.Id,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                Amount = budget.Amount,
                TotalSpent = totalSpent,
                CategorySpendingLimits = categorySpendingLimits
            };
        }
        public bool BudgetOverlapsAsync()
        {
            throw new NotImplementedException();
        }

        public bool BudgetExistsForExpenseDateAsync(DateTime date, ClaimsPrincipal user)
        {
            var budget = _context
                .BelongsToUser<Budget>(user)
                .FirstOrDefaultAsync(b => b.StartDate >= date && b.EndDate <= date);

            return budget is not null;
        }
    }

    public interface IExpenseManager
    {
        public Task<BudgetDetailsDto?> CalculateBudgetStatsAsync(int id, ClaimsPrincipal user);
        public bool BudgetOverlapsAsync();
        public bool BudgetExistsForExpenseDateAsync(DateTime date, ClaimsPrincipal user);
    }
}
