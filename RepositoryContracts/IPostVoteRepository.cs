using Entities;

namespace RepositoryContracts;

public interface IPostVoteRepository
{
    Task<PostVote> AddAsync(PostVote postVote);
    Task UpdateAsync(PostVote postVote);
    Task DeleteAsync(int id);
    Task<PostVote> GetSingleAsync(int id);
    IQueryable<PostVote> GetMany();
}
