using System.Text.Json.Serialization;
using Recipes.Domain;

namespace Recipes.Adapters.Web.Dto;

internal enum IngredientCategoryDto
{
    [JsonStringEnumMemberName("vegetables")]
    Vegetables,

    [JsonStringEnumMemberName("fruits_and_berries")]
    FruitsAndBerries,

    [JsonStringEnumMemberName("meat_and_poultry")]
    MeatAndPoultry,

    [JsonStringEnumMemberName("fish_and_seafood")]
    FishAndSeafood,

    [JsonStringEnumMemberName("dairy_and_eggs")]
    DairyAndEggs,

    [JsonStringEnumMemberName("grains_and_cereals")]
    GrainsAndCereals,

    [JsonStringEnumMemberName("legumes")]
    Legumes,

    [JsonStringEnumMemberName("nuts_and_seeds")]
    NutsAndSeeds,

    [JsonStringEnumMemberName("oils_and_fats")]
    OilsAndFats,

    [JsonStringEnumMemberName("spices_and_seasonings")]
    SpicesAndSeasonings,

    [JsonStringEnumMemberName("sauces_and_pastes")]
    SaucesAndPastes,

    [JsonStringEnumMemberName("bakery_and_sweets")]
    BakeryAndSweets,

    [JsonStringEnumMemberName("other")]
    Other,
}

internal static class IngredientCategoryDtoExtensions
{
    private static readonly Dictionary<IngredientCategory, IngredientCategoryDto> ToDtoMap = new()
    {
        [IngredientCategory.Vegetables]         = IngredientCategoryDto.Vegetables,
        [IngredientCategory.FruitsAndBerries]   = IngredientCategoryDto.FruitsAndBerries,
        [IngredientCategory.MeatAndPoultry]     = IngredientCategoryDto.MeatAndPoultry,
        [IngredientCategory.FishAndSeafood]     = IngredientCategoryDto.FishAndSeafood,
        [IngredientCategory.DairyAndEggs]       = IngredientCategoryDto.DairyAndEggs,
        [IngredientCategory.GrainsAndCereals]   = IngredientCategoryDto.GrainsAndCereals,
        [IngredientCategory.Legumes]            = IngredientCategoryDto.Legumes,
        [IngredientCategory.NutsAndSeeds]       = IngredientCategoryDto.NutsAndSeeds,
        [IngredientCategory.OilsAndFats]        = IngredientCategoryDto.OilsAndFats,
        [IngredientCategory.SpicesAndSeasonings]= IngredientCategoryDto.SpicesAndSeasonings,
        [IngredientCategory.SaucesAndPastes]    = IngredientCategoryDto.SaucesAndPastes,
        [IngredientCategory.BakeryAndSweets]    = IngredientCategoryDto.BakeryAndSweets,
        [IngredientCategory.Other]              = IngredientCategoryDto.Other,
    };

    private static readonly Dictionary<IngredientCategoryDto, IngredientCategory> ToDomainMap =
        ToDtoMap.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static IngredientCategoryDto ToDto(this IngredientCategory category) =>
        ToDtoMap[category];

    public static IngredientCategory ToDomain(this IngredientCategoryDto dto) =>
        ToDomainMap[dto];
}
