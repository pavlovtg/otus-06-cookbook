namespace Recipes.Adapters.Web.Dto;

/// <summary>
/// Позиция пользователя в рейтинге (admin-поле дашборда).
/// AverageRating — средний рейтинг рецептов пользователя (null если рецептов нет).
/// </summary>
internal sealed record UserRankDto(
    Guid Id,
    string DisplayName,
    float? AverageRating,
    int CommentCount);
