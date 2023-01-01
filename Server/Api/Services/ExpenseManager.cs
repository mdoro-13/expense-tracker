using Api.Domain.Entities;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Extensions;
using Bogus.DataSets;
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
            var budget = await GetBudgetByIdAsync(id, user);

            if (budget is null)
            {
                return default;
            }

            decimal totalSpent = 0;
            var categorySpendingLimits = new List<CategorySpendingLimit>();

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

        public async Task<bool> BudgetOverlapsAsync(DateTime startDate, DateTime endDate, ClaimsPrincipal user, int existingBudgetId = 0)
        {
            var query = _context
                .BelongsToUser<Budget>(user);

            if (existingBudgetId > 0)
            {
                query = query
                    .Where(b => b.Id != existingBudgetId);
            }

            var budgets = await query.ToListAsync();

            foreach (var budget in budgets)
            {
                if (startDate >= budget.StartDate && startDate <= budget.EndDate)
                {
                    return true;
                }

                if (endDate >= budget.StartDate && endDate <= budget.EndDate)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> BudgetExistsForExpenseDateAsync(DateTime date, ClaimsPrincipal user)
        {
            var budget = await _context
                .BelongsToUser<Budget>(user)
                .FirstOrDefaultAsync(b => b.StartDate <= date && b.EndDate >= date);

            return budget is not null;
        }

        private async Task<Budget?> GetBudgetByIdAsync(int id, ClaimsPrincipal user)
        {
            return await _context
                .BelongsToUser<Budget>(user)
                .Include(b => b.SpendingLimits)
                    .ThenInclude(x => x.Category)
                        .ThenInclude(x => x.Expenses)
                .Where(b => b.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }

    public interface IExpenseManager
    {
        public Task<BudgetDetailsDto?> CalculateBudgetStatsAsync(int id, ClaimsPrincipal user);
        /// <summary>
        /// Explicitly pass the existing budget id only if the check is done when updating a budget.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="user"></param>
        /// <param name="existingBudgetId"></param>
        /// <returns></returns>
        public Task<bool> BudgetOverlapsAsync(DateTime startDate, DateTime endDate, ClaimsPrincipal user, int existingBudgetId = 0);
        public Task<bool> BudgetExistsForExpenseDateAsync(DateTime date, ClaimsPrincipal user);
    }
}
