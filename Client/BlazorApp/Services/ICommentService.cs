using ApiContracts.Comments;
namespace BlazorApp.Services;

public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto request);
    Task<List<CommentDto>> GetCommentsForPostAsync(int postId);
}