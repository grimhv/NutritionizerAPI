using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Honeypox.Nutritionizer.Data;
using AutoMapper;
using Honeypox.Nutritionizer.Dtos;

namespace Honeypox.Nutritionizer.Controllers
{
    [Route("api/recipe")]
    [ApiController]
    public class RecipeController : ControllerBase // Inherit from ControllerBase, which excludes "Views"
    {
        private readonly IRecipeAPIRepo _repository;
        private readonly IMapper _mapper;

        // This utilizes dependency injection to _actually_ use `SqlRecipeAPIRepo` instead of `IRecipeAPIRepo`
        public RecipeController(IRecipeAPIRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RecipeReadDto>> GetAllRecipes()
        {
            var recipeItems = _repository.GetAllRecipes();
            return Ok(_mapper.Map<IEnumerable<RecipeReadDto>>(recipeItems));
        }

        [HttpGet("{id}")]
        public ActionResult<RecipeReadDto> GetRecipeById(int id)
        {
            var recipeItem = _repository.GetRecipeById(id);
            if (recipeItem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<RecipeReadDto>(recipeItem));
        }
    }
}