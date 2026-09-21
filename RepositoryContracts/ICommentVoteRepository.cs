using Entities;

namespace RepositoryContracts;

public interface ICommentVoteRepository
{
    Task<CommentVote> AddAsync(CommentVote commentVote);
    Task UpdateAsync(CommentVote commentVote);
    Task DeleteAsync(int id);
    Task<CommentVote> GetSingleAsync(int id);
    IQueryable<CommentVote> GetMany();
}
