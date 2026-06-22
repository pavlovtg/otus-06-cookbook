using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class RecipeCommentService : IRecipeCommentService
{
    private readonly IRecipeRepository _repository;

    public RecipeCommentService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CommentDetail>> GetCommentsAsync(
        RecipeId recipeId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _ = await _repository.GetByIdAsync(recipeId, cancellationToken)
            ?? throw new RecipeNotFoundException(recipeId);

        return await _repository.GetCommentsPagedAsync(recipeId, page, pageSize, cancellationToken);
    }

    public async Task<CommentDetail> AddCommentAsync(
        RecipeId recipeId,
        UserId authorId,
        string text,
        CancellationToken cancellationToken = default)
    {
        _ = await _repository.GetByIdAsync(recipeId, cancellationToken)
            ?? throw new RecipeNotFoundException(recipeId);

        var comment = RecipeComment.Create(RecipeCommentId.New(), recipeId, authorId, text);

        await _repository.AddCommentAsync(comment, cancellationToken);
        await _repository.CommitAsync(cancellationToken);

        return await _repository.GetCommentAsync(comment.Id, cancellationToken)
            ?? throw new CommentNotFoundException();
    }

    public async Task DeleteCommentAsync(
        RecipeCommentId commentId,
        UserId currentUserId,
        UserRole? currentUserRole,
        CancellationToken cancellationToken = default)
    {
        var comment = await _repository.GetCommentAsync(commentId, cancellationToken)
            ?? throw new CommentNotFoundException();

        var recipe = await _repository.GetByIdAsync(comment.RecipeId, cancellationToken);

        if (!CanDeleteComment(comment, recipe, currentUserId, currentUserRole))
            throw new CommentForbiddenException();

        await _repository.DeleteCommentAsync(commentId, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    private static bool CanDeleteComment(CommentDetail comment, Recipe? recipe, UserId currentUserId, UserRole? currentUserRole)
    {
        if (currentUserRole == UserRole.Admin)
            return true;

        if (comment.AuthorId == currentUserId)
            return true;

        if (recipe?.AuthorId == currentUserId)
            return true;

        return false;
    }
}
