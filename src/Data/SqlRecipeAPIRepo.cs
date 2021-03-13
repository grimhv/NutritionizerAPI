using System;
using System.Collections.Generic;
using System.Linq;
using Honeypox.Nutritionizer.Classes;
using Honeypox.Nutritionizer.Models;

namespace Honeypox.Nutritionizer.Data
{
    public class SqlRecipeAPIRepo : IRecipeAPIRepo
    {
        private readonly RecipeContext _context;
        public SqlRecipeAPIRepo(RecipeContext context)
        {
            _context = context;
        }

        public void CreateRecipe(Recipe cmd)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteRecipe(Recipe cmd)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Recipe> GetAllRecipes()
        {
            // return all recipes
            return _context.Recipe.ToList();
        }

        public Recipe GetRecipeById(int id)
        {
            return ApiLibrary.GetRecipe(id, _context);
        }
        public bool SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateRecipe(Recipe cmd)
        {
            throw new System.NotImplementedException();
        }
    }
}