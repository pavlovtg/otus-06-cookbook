using System.Text.Json.Serialization;

namespace Recipes.Adapters.Web.Dto;

[JsonConverter(typeof(JsonStringEnumConverter<CategoryTypeDto>))]
internal enum CategoryTypeDto
{
    [JsonStringEnumMemberName("meal_role")]
    MealRole,

    [JsonStringEnumMemberName("cooking_method")]
    CookingMethod,

    [JsonStringEnumMemberName("main_ingredient")]
    MainIngredient,

    [JsonStringEnumMemberName("cuisine")]
    Cuisine,

    [JsonStringEnumMemberName("meal_time")]
    MealTime,

    [JsonStringEnumMemberName("dietary")]
    Dietary,

    [JsonStringEnumMemberName("serving_form")]
    ServingForm,
}
