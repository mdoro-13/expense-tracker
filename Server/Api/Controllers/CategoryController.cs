using Api.Core.Validation.DTO;
using Api.Domain.Entities;
using Api.DTO.Request;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Extensions;
using Api.Utils.Helpers;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public class CategoryController : BaseApiController
{
	private readonly DataContext _context;
	public CategoryController(DataContext context)
	{
		_context = context;
	}

	[HttpGet]
	[ProducesResponseType(typeof(ICollection<CategoryReadDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> Get()
	{
		List<Category> categories = await GetUserCategoriesAsync();

		return Ok(categories.Adapt<ICollection<CategoryReadDto>>());
	}

	[HttpGet("{id:int}", Name = "GetCategory")]
	[ProducesResponseType(typeof(CategoryReadDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		var category = await GetCategoryByIdAsync(id);

		if (category is null)
		{
			return NotFound();
		}

		return Ok(category.Adapt<CategoryReadDto>());
	}

	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[HttpPost]
	public async Task<IActionResult> Post(CategoryCreateDto categoryCreateDto)
	{
		var userCategories = await GetUserCategoriesAsync();
		var categoryExists = userCategories.Any(c => c.Name.ToLower().Trim() == categoryCreateDto.Name.ToLower().Trim());

		if (categoryExists)
		{
			return BadRequest();
		}

		var category = categoryCreateDto.Adapt<Category>();
		category.UserId = JwtUtils.GetUserId(User)!;

		await _context.Set<Category>().AddAsync(category);

		if (await _context.SaveChangesAsync() <= 0)
		{
			return BadRequest();
		}

		return CreatedAtRoute("GetCategory", new { Id = category.Id }, category.Adapt<CategoryReadDto>());

	}

	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[HttpPatch("{id:int}")]
	public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Category> categoryPatchDto)
	{
		var categoryToUpdate = await GetCategoryByIdAsync(id);

		if (categoryToUpdate is null)
		{
			return NotFound();
		}

        categoryPatchDto.ApplyTo(categoryToUpdate);

		var validator = new CategoryCreateDtoValidator();
		var result = await validator.ValidateAsync(categoryToUpdate.Adapt<CategoryCreateDto>());
		if (!result.IsValid)
		{
			var errors = new List<string>();
			foreach (var error in result.Errors)
			{
				errors.Add(error.ErrorMessage);
			}

			return BadRequest(new { errors });
		}

		_context.Entry(categoryToUpdate).Property(x => x.UserId).IsModified = false;

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
        var categoryToDelete = await GetCategoryByIdAsync(id);

		if (categoryToDelete is null)
		{
			return NotFound();
		}

		_context.Set<Category>().Remove(categoryToDelete);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return BadRequest();
        }

        return NoContent();
    }

    private async Task<List<Category>> GetUserCategoriesAsync()
    {
        return await _context
            .BelongsToUser<Category>(User)
            .ToListAsync();
    }

    private async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context
            .BelongsToUser<Category>(User)
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }


}
