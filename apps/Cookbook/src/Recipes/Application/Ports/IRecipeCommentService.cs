using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipeCommentService
{
    Task<PagedResult<CommentDetail>> GetCommentsAsync(RecipeId recipeId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<CommentDetail> AddCommentAsync(RecipeId recipeId, UserId authorId, string text, CancellationToken cancellationToken = default);
    Task DeleteCommentAsync(RecipeCommentId commentId, UserId currentUserId, UserRole? currentUserRole, CancellationToken cancellationToken = default);
}
