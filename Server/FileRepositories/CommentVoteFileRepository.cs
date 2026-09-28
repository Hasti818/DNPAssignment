using Entities;
using RepositoryContracts;
using System.Text.Json;

namespace FileRepositories;

public class CommentVoteFileRepository : ICommentVoteRepository
{
    private readonly string filePath = "commentVotes.json";

    public CommentVoteFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<CommentVote> AddAsync(CommentVote commentVote)
    {
        string commentVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<CommentVote> commentVotes =
            JsonSerializer.Deserialize<List<CommentVote>>(commentVotesAsJson)!;

        int maxId = commentVotes.Count > 0
            ? commentVotes.Max(vote => vote.Id)
            : 0;

        commentVote.Id = maxId + 1;

        commentVotes.Add(commentVote);

        commentVotesAsJson = JsonSerializer.Serialize(commentVotes);

        await File.WriteAllTextAsync(filePath, commentVotesAsJson);

        return commentVote;
    }

    public async Task UpdateAsync(CommentVote commentVote)
    {
        string commentVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<CommentVote> commentVotes =
            JsonSerializer.Deserialize<List<CommentVote>>(commentVotesAsJson)!;

        CommentVote? existingCommentVote =
            commentVotes.SingleOrDefault(vote => vote.Id == commentVote.Id);

        if (existingCommentVote is null)
        {
            throw new InvalidOperationException(
                $"CommentVote with ID '{commentVote.Id}' not found");
        }

        commentVotes.Remove(existingCommentVote);
        commentVotes.Add(commentVote);

        commentVotesAsJson = JsonSerializer.Serialize(commentVotes);

        await File.WriteAllTextAsync(filePath, commentVotesAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string commentVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<CommentVote> commentVotes =
            JsonSerializer.Deserialize<List<CommentVote>>(commentVotesAsJson)!;

        CommentVote? commentVoteToRemove =
            commentVotes.SingleOrDefault(vote => vote.Id == id);

        if (commentVoteToRemove is null)
        {
            throw new InvalidOperationException(
                $"CommentVote with ID '{id}' not found");
        }

        commentVotes.Remove(commentVoteToRemove);

        commentVotesAsJson = JsonSerializer.Serialize(commentVotes);

        await File.WriteAllTextAsync(filePath, commentVotesAsJson);
    }

    public async Task<CommentVote> GetSingleAsync(int id)
    {
        string commentVotesAsJson = await File.ReadAllTextAsync(filePath);

        List<CommentVote> commentVotes =
            JsonSerializer.Deserialize<List<CommentVote>>(commentVotesAsJson)!;

        CommentVote? commentVote =
            commentVotes.SingleOrDefault(vote => vote.Id == id);

        if (commentVote is null)
        {
            throw new InvalidOperationException(
                $"CommentVote with ID '{id}' not found");
        }

        return commentVote;
    }

    public IQueryable<CommentVote> GetMany()
    {
        string commentVotesAsJson =
            File.ReadAllTextAsync(filePath).Result;

        List<CommentVote> commentVotes =
            JsonSerializer.Deserialize<List<CommentVote>>(commentVotesAsJson)!;

        return commentVotes.AsQueryable();
    }
}