/* What Is an Interface?
 * An interface is just as it sounds; it’s a specification for what functionality we want it to
 * provide (in this case to our Controller), but we don’t detail how it will be provided –
 * that comes later. It’s essentially an agreement, or contract, with the consumer of that Interface.
 */

using System.Collections.Generic;
using Honeypox.Nutritionizer.Models;

namespace Honeypox.Nutritionizer.Data
{
    public interface IRecipeAPIRepo
    {
        bool SaveChanges ();
        IEnumerable<Recipe> GetAllRecipes ();
        //Recipe GetRecipeById (int id);
        Recipe GetRecipeById (int id);
        void CreateRecipe (Recipe cmd);
        void UpdateRecipe (Recipe cmd);
        void DeleteRecipe (Recipe cmd);
    }
}