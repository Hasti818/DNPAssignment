using Entities;
using RepositoryContracts;
using System.Text.Json;

namespace FileRepositories;

public class PostVoteFileRepository : IPostVoteRepository
{
    private readonly string filePath = "postVotes.json";

    public PostVoteFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<PostVote> AddAsync(PostVote postVote)
    {
        string postVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<PostVote> postVotes =
            JsonSerializer.Deserialize<List<PostVote>>(postVotesAsJson)!;

        int maxId = postVotes.Count > 0
            ? postVotes.Max(vote => vote.Id)
            : 0;

        postVote.Id = maxId + 1;

        postVotes.Add(postVote);

        postVotesAsJson = JsonSerializer.Serialize(postVotes);

        await File.WriteAllTextAsync(filePath, postVotesAsJson);

        return postVote;
    }

    public async Task UpdateAsync(PostVote postVote)
    {
        string postVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<PostVote> postVotes =
            JsonSerializer.Deserialize<List<PostVote>>(postVotesAsJson)!;

        PostVote? existingPostVote =
            postVotes.SingleOrDefault(vote => vote.Id == postVote.Id);

        if (existingPostVote is null)
        {
            throw new InvalidOperationException(
                $"PostVote with ID '{postVote.Id}' not found");
        }

        postVotes.Remove(existingPostVote);
        postVotes.Add(postVote);

        postVotesAsJson = JsonSerializer.Serialize(postVotes);

        await File.WriteAllTextAsync(filePath, postVotesAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string postVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<PostVote> postVotes =
            JsonSerializer.Deserialize<List<PostVote>>(postVotesAsJson)!;

        PostVote? postVoteToRemove =
            postVotes.SingleOrDefault(vote => vote.Id == id);

        if (postVoteToRemove is null)
        {
            throw new InvalidOperationException(
                $"PostVote with ID '{id}' not found");
        }

        postVotes.Remove(postVoteToRemove);

        postVotesAsJson = JsonSerializer.Serialize(postVotes);

        await File.WriteAllTextAsync(filePath, postVotesAsJson);
    }

    public async Task<PostVote> GetSingleAsync(int id)
    {
        string postVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<PostVote> postVotes =
            JsonSerializer.Deserialize<List<PostVote>>(postVotesAsJson)!;

        PostVote? postVote =
            postVotes.SingleOrDefault(vote => vote.Id == id);

        if (postVote is null)
        {
            throw new InvalidOperationException(
                $"PostVote with ID '{id}' not found");
        }

        return postVote;
    }

    public IQueryable<PostVote> GetMany()
    {
        string postVotesAsJson =
            File.ReadAllTextAsync(filePath).Result;

        List<PostVote> postVotes =
            JsonSerializer.Deserialize<List<PostVote>>(postVotesAsJson)!;

        return postVotes.AsQueryable();
    }
}