using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new();
    public CommentInMemoryRepository()
    {
        comments.Add(new Comment
        {
            Id = 1,
            Body = "Nice post!",
            UserId = 2,
            PostId = 1,
            CreatedAt = DateTime.Now
        });

        comments.Add(new Comment
        {
            Id = 2,
            Body = "I really like C#.",
            UserId = 1,
            PostId = 2,
            CreatedAt = DateTime.Now
        });

        comments.Add(new Comment
        {
            Id = 3,
            Body = "Java is one of my favourites.",
            UserId = 3,
            PostId = 3,
            CreatedAt = DateTime.Now
        });
    }
    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(comment => comment.Id) + 1
            : 1;

        comments.Add(comment);

        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment =
            comments.SingleOrDefault(existingComment => existingComment.Id == comment.Id);

        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove =
            comments.SingleOrDefault(comment => comment.Id == id);

        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);

        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment =
            comments.SingleOrDefault(comment => comment.Id == id);

        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
}