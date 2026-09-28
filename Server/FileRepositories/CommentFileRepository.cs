using Entities;
using RepositoryContracts;
using System.Text.Json;
namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        int maxId = comments.Count > 0
            ? comments.Max(comment => comment.Id)
            : 0;

        comment.Id = maxId + 1;

        comments.Add(comment);

        commentsAsJson = JsonSerializer.Serialize(comments);

        await File.WriteAllTextAsync(filePath, commentsAsJson);

        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        Comment? existingComment =
            comments.SingleOrDefault(existingComment => existingComment.Id == comment.Id);
        if (existingComment != null)
        {
            throw  new InvalidOperationException("Comment is not found");
        }
        comments.Remove(existingComment);
        comments.Add(comment);
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        Comment?  commentToRemove=
            comments.SingleOrDefault(comment => comment.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException("Comment is not found");
        }
        comments.Remove(commentToRemove);
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        string commentsAsJson=await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        Comment? comment =
            comments.SingleOrDefault(comment => comment.Id == id);
        if(comment is null){
            throw new InvalidOperationException("Comment is not found");
        }
        return comment;
    }
    public IQueryable<Comment> GetMany()
    {
        string commentsAsJson = File.ReadAllTextAsync(filePath).Result;

        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;

        return comments.AsQueryable();
    }

}