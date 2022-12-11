using Api.Domain.Entities;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Services;
using Api.Utils.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class BudgetController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IExpenseManager _expenseManager;
        public BudgetController(DataContext context, IExpenseManager expenseManager)
        {
            _context = context;
            _expenseManager = expenseManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<BudgetReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // TODO: add filter by date support (e.g fetch budgets for 2022 only)

            var budgets = await _context
                .BelongsToUser<Budget>(User)
                .ToListAsync();

            return Ok(budgets.Adapt<ICollection<BudgetReadDto>>());
        }

        [HttpGet("{id:int}", Name = "GetBudget")]
        [ProducesResponseType(typeof(BudgetDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var budgetStats = await _expenseManager.CalculateBudgetStatsAsync(id, User);

            if (budgetStats is null)
            {
                return NotFound();
            }

            return Ok(budgetStats);
        }
    }
}
