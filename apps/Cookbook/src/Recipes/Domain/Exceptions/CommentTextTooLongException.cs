namespace Recipes.Domain.Exceptions;

internal sealed class CommentTextTooLongException(int length) : CommentDomainException()
{
    public int Length { get; } = length;
}
