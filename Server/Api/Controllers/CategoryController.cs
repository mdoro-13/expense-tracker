using Api.Domain.Entities;
using Api.DTO.Response;
using Api.Infrastructure.Data;
using Api.Utils.Helpers;
using Mapster;
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
	public async Task<IActionResult> Get()
	{
		var categories = await _context.Set<Category>()
			.Where(c => c.UserId == JwtUtils.GetUserId(User))
			.ToListAsync();

		return Ok(categories.Adapt<ICollection<CategoryReadDto>>());
	}


}
