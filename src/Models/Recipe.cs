using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Honeypox.Nutritionizer.Models
{
    [Table("recipes")]
    public class Recipe
    {
        [Key]
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [Required]
        [Column("web_id")]
        public int WebId { get; set; }
        
        [Required]
        [Column("rating")]
        public float Rating { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("website")]
        public string Website { get; set; }

        [Required]
        [MaxLength(256)]
        [Column("url")]
        public string Url { get; set; }

        
        public List<Ingredient> Ingredients { get; set; }
    }
}