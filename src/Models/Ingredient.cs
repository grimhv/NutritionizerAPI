using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Honeypox.Nutritionizer.Models
{
    [Table("ingredients")]
    public class Ingredient
    {
        public Ingredient(int IngredientId, int RecipeId, int Quantity, string Description, string RecipeSection, string Unit)
        {
            this.IngredientId = IngredientId;
            this.RecipeId = RecipeId;
            this.Quantity = Quantity;
            this.Description = Description;
            this.RecipeSection = RecipeSection;
            this.Unit = Unit;
        }
        [Key]
        [Column("ingredient_id")]
        public int IngredientId { get; set; }

        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(64)]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("recipe_section")]
        public string RecipeSection { get; set; }

        [Required]
        [MaxLength(16)]
        [Column("unit")]
        public string Unit { get; set; }
    }
}