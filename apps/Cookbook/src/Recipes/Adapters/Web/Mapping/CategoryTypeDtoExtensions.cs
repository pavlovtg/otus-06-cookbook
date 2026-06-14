using Recipes.Adapters.Web.Dto;
using Recipes.Domain;

namespace Recipes.Adapters.Web.Mapping;

internal static class CategoryTypeDtoExtensions
{
    private static readonly Dictionary<CategoryType, CategoryTypeDto> ToDtoMap = new()
    {
        [CategoryType.MealRole] = CategoryTypeDto.MealRole,
        [CategoryType.CookingMethod] = CategoryTypeDto.CookingMethod,
        [CategoryType.MainIngredient] = CategoryTypeDto.MainIngredient,
        [CategoryType.Cuisine] = CategoryTypeDto.Cuisine,
        [CategoryType.MealTime] = CategoryTypeDto.MealTime,
        [CategoryType.Dietary] = CategoryTypeDto.Dietary,
        [CategoryType.ServingForm] = CategoryTypeDto.ServingForm,
    };

    private static readonly Dictionary<CategoryTypeDto, CategoryType> ToDomainMap =
        ToDtoMap.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static CategoryTypeDto ToDto(this CategoryType type) => ToDtoMap[type];

    public static CategoryType ToDomain(this CategoryTypeDto dto) => ToDomainMap[dto];
}
