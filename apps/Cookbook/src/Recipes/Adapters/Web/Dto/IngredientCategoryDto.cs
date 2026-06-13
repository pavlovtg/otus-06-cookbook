using System.Text.Json.Serialization;

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
