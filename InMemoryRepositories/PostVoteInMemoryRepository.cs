using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostVoteInMemoryRepository : IPostVoteRepository
{
    private readonly List<PostVote> postVotes = new();
    public PostVoteInMemoryRepository()
    {
        postVotes.Add(new PostVote
        {
            Id = 1,
            UserId = 1,
            PostId = 2,
            IsLike = true
        });

        postVotes.Add(new PostVote
        {
            Id = 2,
            UserId = 2,
            PostId = 1,
            IsLike = true
        });

        postVotes.Add(new PostVote
        {
            Id = 3,
            UserId = 3,
            PostId = 1,
            IsLike = false
        });
    }
    
    public Task<PostVote> AddAsync(PostVote postVote)
    {
        postVote.Id = postVotes.Any()
            ? postVotes.Max(vote => vote.Id) + 1
            : 1;

        postVotes.Add(postVote);

        return Task.FromResult(postVote);
    }

    public Task UpdateAsync(PostVote postVote)
    {
        PostVote? existingPostVote =
            postVotes.SingleOrDefault(vote => vote.Id == postVote.Id);

        if (existingPostVote is null)
        {
            throw new InvalidOperationException(
                $"PostVote with ID '{postVote.Id}' not found");
        }

        postVotes.Remove(existingPostVote);
        postVotes.Add(postVote);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        PostVote? postVoteToRemove =
            postVotes.SingleOrDefault(vote => vote.Id == id);

        if (postVoteToRemove is null)
        {
            throw new InvalidOperationException(
                $"PostVote with ID '{id}' not found");
        }

        postVotes.Remove(postVoteToRemove);

        return Task.CompletedTask;
    }

    public Task<PostVote> GetSingleAsync(int id)
    {
        PostVote? postVote =
            postVotes.SingleOrDefault(vote => vote.Id == id);

        if (postVote is null)
        {
            throw new InvalidOperationException(
                $"PostVote with ID '{id}' not found");
        }

        return Task.FromResult(postVote);
    }

    public IQueryable<PostVote> GetMany()
    {
        return postVotes.AsQueryable();
    }

    public Task<Post> getByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}