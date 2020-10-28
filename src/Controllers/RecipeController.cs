using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Honeypox.Nutritionizer.Data;
using Honeypox.Nutritionizer.Models;
using Honeypox.Nutritionizer.Classes;
using Microsoft.Extensions.Options;

namespace Honeypox.Nutritionizer.Controllers
{
    [Route("api/recipe")]
    [ApiController]
    public class RecipeController : ControllerBase // Inherit from ControllerBase, which excludes "Views"
    {
        private readonly IRecipeAPIRepo _repository;

        // This utilizes dependency injection to _actually_ use `SqlRecipeAPIRepo` instead of `IRecipeAPIRepo`
        public RecipeController(IRecipeAPIRepo repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<List<string>> GetAllRecipes()
        {
            var recipeItems = _repository.GetAllRecipes();
            return Ok(recipeItems);
        }

        [HttpGet("{id}")]
        public ActionResult<Recipe> GetRecipeById(int id)
        {
            var recipeItem = _repository.GetRecipeById(id);
            if (recipeItem == null)
            {
                return NotFound();
            }
            return Ok(recipeItem);
        }
    }
}