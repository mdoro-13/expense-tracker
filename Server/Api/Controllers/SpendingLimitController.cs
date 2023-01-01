using Api.Domain.Entities;
using Api.DTO.Request;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Extensions;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public class SpendingLimitController : BaseApiController
{
    private readonly DataContext _context;
    public SpendingLimitController(DataContext context)
    {
        _context = context;
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post(SpendingLimitCreateDto dto)
    {
        var budgetExists = (await _context.BelongsToUser<Budget>(User)
            .Where(b => b.Id == dto.BudgetId)
            .FirstOrDefaultAsync()) is not null;

        if (!budgetExists)
        {
            return BadRequest("The budget does not exist");
        }

        var categoryExists = (await _context.BelongsToUser<Category>(User)
            .Where(b => b.Id == dto.CategoryId)
            .FirstOrDefaultAsync()) is not null;

        if (!categoryExists)
        {
            return BadRequest("The category does not exist");
        }
        var spendingLimit = dto.Adapt<SpendingLimit>();

        await _context.Set<SpendingLimit>()
            .AddAsync(spendingLimit);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<SpendingLimit> expensePatch)
    {
        return Ok();
    }
}
