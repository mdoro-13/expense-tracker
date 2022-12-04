using Api.Core.Validation.DTO;
using Api.Domain.Entities;
using Api.DTO.Request;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Extensions;
using Api.Utils.Helpers;
using Api.Utils.Query;
using Api.Validation.DTO;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public class ExpenseController : BaseApiController
{
	private readonly DataContext _context;
	public ExpenseController(DataContext context)
	{
		_context = context;
	}

	[HttpGet]
	[ProducesResponseType(typeof(ICollection<ExpenseReadDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> Get([FromQuery] ExpenseFilter filter, [FromQuery] ExpenseSort sort, [FromQuery] ExpensePaging paging)
	{
		var query = _context
			.BelongsToUser<Expense>(User)
			.Include(e => e.Category)
			.Where(e => e.Date >= filter.DateFrom && e.Date <= filter.DateTo);
		
		if (filter.CategoryIds.Any())
		{
			query = query.Where(e => filter.CategoryIds.Contains(e.CategoryId));
		}

		switch(sort.SortBy)
		{
			case "date":
				query = sort.Direction > 0 ? query.OrderBy(e => e.Date) : query.OrderByDescending(e => e.Date);
				break;
			
			case "categoryName":
				query = sort.Direction > 0 ? query.OrderBy(e => e.Category.Name) : query.OrderByDescending(e => e.Category.Name);
				break;
			
			default:
				query = sort.Direction > 0 ? query.OrderBy(e => e.Date) : query.OrderByDescending(e => e.Date);
				break;
		}

		var pagedExpenses = await PagedList<ExpenseReadDto>
			.CreateAsync(query.ProjectToType<ExpenseReadDto>().AsNoTracking(), paging.PageNumber, paging.PageSize);

        Response.AddPaginationHeader(pagedExpenses.CurrentPage, pagedExpenses.PageSize, pagedExpenses.TotalCount, pagedExpenses.TotalPages);
		
		return Ok(pagedExpenses);
	}

	[HttpGet("{id:int}", Name = "GetExpense")]
	[ProducesResponseType(typeof(ExpenseReadDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		var expense = await GetExpenseByIdAsync(id);

		if (expense is null)
		{
			return NotFound();
		}

		return Ok(expense.Adapt<ExpenseReadDto>());
	}


	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[HttpPost]
	public async Task<IActionResult> Post(ExpenseCreateDto dto)
	{
		// TODO: check if expense belongs to a budget
		// Otherwise, adding an expense should not be possible

		if (dto.CategoryId is not null && !(await ExpenseCategoryBelongsToUserAsync(dto.CategoryId)))
		{
			return BadRequest();
		}

		var expense = dto.Adapt<Expense>();
		expense.UserId = JwtUtils.GetUserId(User)!;

		await _context.Set<Expense>().AddAsync(expense);

		if (await _context.SaveChangesAsync() <= 0)
		{
			return BadRequest();
		}

		return CreatedAtRoute("GetExpense", new { Id = expense.Id }, expense.Adapt<ExpenseReadDto>());
	}

	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[HttpPatch("{id:int}")]
	public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Expense> expensePatch)
	{
		var expenseToUpdate = await GetExpenseByIdAsync(id);

            if (expenseToUpdate is null)
		{
			return NotFound();
		}

		expensePatch.ApplyTo(expenseToUpdate);


        if (expenseToUpdate.CategoryId is not null && !(await ExpenseCategoryBelongsToUserAsync(expenseToUpdate.CategoryId)))
        {
			return BadRequest();
        }

        var validator = new ExpenseCreateDtoValidator();

        var result = await validator.ValidateAsync(expenseToUpdate.Adapt<ExpenseCreateDto>());
        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.ErrorMessage);
            }

            return BadRequest(new { errors });
        }

        _context.Entry(expenseToUpdate).Property(x => x.UserId).IsModified = false;

        if (await _context.SaveChangesAsync() <= 0)
        {
            return BadRequest();
        }

        return NoContent();
	}

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var expenseToDelete = await GetExpenseByIdAsync(id);

		if (expenseToDelete is null)
		{
			return NotFound();
		}

		_context.Set<Expense>().Remove(expenseToDelete);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return BadRequest();
        }

        return NoContent();

    }

    private async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        return await _context
            .BelongsToUser<Expense>(User)
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    private async Task<bool> ExpenseCategoryBelongsToUserAsync(int? categoryId)
    {
        return await _context.BelongsToUser<Category>(User)
            .Where(c => c.Id == categoryId)
            .FirstOrDefaultAsync() is not null;
    }
}
