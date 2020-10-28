using Microsoft.EntityFrameworkCore;
using Honeypox.Nutritionizer.Models;

namespace Honeypox.Nutritionizer.Data
{
    public class RecipeContext : DbContext
    {
        public RecipeContext(DbContextOptions<RecipeContext> options)
            : base(options)
            { 

            }
            public DbSet<Recipe> Recipe { get; set; }
            public DbSet<Ingredient> Ingredient { get; set; }
    }
}