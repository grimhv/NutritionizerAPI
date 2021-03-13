using AutoMapper;
using Honeypox.Nutritionizer.Dtos;
using Honeypox.Nutritionizer.Models;

namespace Honeypox.Nutritionizer.Profiles
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Recipe, RecipeReadDto>();
        }
    }
}