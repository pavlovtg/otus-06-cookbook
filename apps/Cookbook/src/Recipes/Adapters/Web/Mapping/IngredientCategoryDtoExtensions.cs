using System.Text.Json;
using Recipes.Adapters.Web.Dto;
using Recipes.Domain;

namespace Recipes.Adapters.Web.Mapping;

internal static class IngredientCategoryDtoExtensions
{
    private static readonly Dictionary<IngredientCategory, IngredientCategoryDto> ToDtoMap = new()
    {
        [IngredientCategory.Vegetables] = IngredientCategoryDto.Vegetables,
        [IngredientCategory.FruitsAndBerries] = IngredientCategoryDto.FruitsAndBerries,
        [IngredientCategory.MeatAndPoultry] = IngredientCategoryDto.MeatAndPoultry,
        [IngredientCategory.FishAndSeafood] = IngredientCategoryDto.FishAndSeafood,
        [IngredientCategory.DairyAndEggs] = IngredientCategoryDto.DairyAndEggs,
        [IngredientCategory.GrainsAndCereals] = IngredientCategoryDto.GrainsAndCereals,
        [IngredientCategory.Legumes] = IngredientCategoryDto.Legumes,
        [IngredientCategory.NutsAndSeeds] = IngredientCategoryDto.NutsAndSeeds,
        [IngredientCategory.OilsAndFats] = IngredientCategoryDto.OilsAndFats,
        [IngredientCategory.SpicesAndSeasonings] = IngredientCategoryDto.SpicesAndSeasonings,
        [IngredientCategory.SaucesAndPastes] = IngredientCategoryDto.SaucesAndPastes,
        [IngredientCategory.BakeryAndSweets] = IngredientCategoryDto.BakeryAndSweets,
        [IngredientCategory.Other] = IngredientCategoryDto.Other,
    };

    private static readonly Dictionary<IngredientCategoryDto, IngredientCategory> ToDomainMap =
        ToDtoMap.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static IngredientCategoryDto ToDto(this IngredientCategory category) =>
        ToDtoMap[category];

    public static string ToDtoString(this IngredientCategory category) =>
        JsonSerializer.Serialize(category.ToDto()).Trim('"');

    public static IngredientCategory ToDomain(this IngredientCategoryDto dto) =>
        ToDomainMap[dto];

    public static bool TryParseCategory(string value, out IngredientCategoryDto result)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<IngredientCategoryDto>($"\"{value}\"");
            result = deserialized;
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
