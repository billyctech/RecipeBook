using Microsoft.AspNetCore.Mvc;
using RecipeBook.ServiceLibrary.Domains;
using RecipeBook.ServiceLibrary.Entities;
using System;
using System.Threading.Tasks;

namespace RecipeBook.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RecipeController : ControllerBase
	{
		//[FromQuery] - Gets values from the query string.
		//[FromRoute] - Gets values from route data.
		//[FromForm] - Gets values from posted form fields.
		//[FromHeader] - Gets values from HTTP headers.
		//[FromBody] - Gets values from the request body.

		[HttpGet("{recipeId}")] // api/recipe/{recipeid}
		public async Task<IActionResult> GetOnceAsync(
			[FromRoute]Guid recipeId)
		{
			return Ok(recipeId);
		}

		[HttpGet] // api/recipes?pageSize=10&pageNumber=1
		public async Task<IActionResult> GetListAsync(
			[FromQuery]int pageSize,
			[FromQuery]int pageNumber)
		{
			return Ok(pageSize + " " + pageNumber);
		}

		[HttpPost]
		public async Task<IActionResult> PostAsync([FromBody] RecipeEntity recipe)
		{
			return Ok(recipe);
		}

		[HttpPut]
		public async Task<IActionResult> PutAsync([FromBody] RecipeEntity recipe)
		{
			return Ok(recipe);
		}

		[HttpDelete("{recipeId}")]
		public async Task<IActionResult> DeleteAsync(Guid recipeId)
		{
			return Ok(recipeId);
		}
	}
}