using Api.Domain.Entities;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class BudgetController : BaseApiController
    {
        private readonly DataContext _context;
        public BudgetController(DataContext context)
        {
            _context = context;
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
    }
}
