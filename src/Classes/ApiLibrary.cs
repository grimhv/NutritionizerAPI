using Honeypox.Nutritionizer.Models;
using Honeypox.Nutritionizer.Data;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;
using Honeypox.GLog;

namespace Honeypox.Nutritionizer.Classes
{
    public class ApiLibrary
    {
        public static Recipe GetRecipe(int id, RecipeContext context)
        {
            Recipe recipe;
            var ingredients = new List<Ingredient>();
            var logMessage = new List<string>();
            Logger logger = new Logger()
            {
                LogPath = @"c:\dev\api_cs_new\n_api.log"
            };

            logger.Debug($"GET Request, id = {id}");

            try
            {
                // Create a list of everything in the `Ingredient` context by referencing the RecipeID
                ingredients = context.Ingredient
                    .Where(i => i.RecipeId == id)
                    .Select(i => i)
                    .ToList();

                // Select everything in the `Recipe` context with the matching RecipeID,
                // and use the Ingredient list from above to complete our data
                recipe = context.Recipe
                    .Where(r => r.RecipeId == id)
                    .Select(r => new Recipe
                    {
                        RecipeId = r.RecipeId,
                            WebId = r.WebId,
                            Rating = r.Rating,
                            Url = r.Url,
                            Website = r.Website,
                            Ingredients = ingredients
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                // Something went wrong, log the error message and move it along
                logMessage.Add("Error when trying to parse the database:");
                logMessage.AddRange(e.ToString().Split("\n").ToList());
                logger.Error(logMessage);
                return null;
            }

            // FirstOrDefault returns `null` if it's "Default" (I.E. it was unable to find "First").  In that case,
            // there were no recipes found with the matching ID
            if (recipe == null)
            {
                // Something went wrong
                logger.Warning($"No recipes found.");
                return null;
            }

            // Format and log the recipe as JSON
            logMessage.Add("Found recipe:");
            logMessage.AddRange(
                JsonSerializer.Serialize(
                    recipe, new JsonSerializerOptions { WriteIndented = true })
                .Split("\n")
                .ToList());

            logger.Debug(logMessage);
            return recipe;
        }
    }
}