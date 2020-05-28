using Microsoft.AspNetCore.Mvc;
using RecipeBook.ServiceLibrary.Domains;
using RecipeBook.ServiceLibrary.Entities;

namespace RecipeBook.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RecipeController : ControllerBase
	{
		[HttpGet]
		public IActionResult AddNewRecipe([FromQuery]RecipeEntity recipeEntity)
		{
			var businessLogic = new Recipe();
			businessLogic.SaveRecipe(recipeEntity);
			return Ok();
		}
	}
}