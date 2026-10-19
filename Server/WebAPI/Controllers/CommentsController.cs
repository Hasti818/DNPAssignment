using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
namespace WebAPI.Controllers;
[ApiController]
[Route("api")]
public class CommentsController:ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    public CommentsController
        (ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }

    [HttpPost("posts/{postId:int}/comments")]
    public async Task<ActionResult<CommentDto>> CreateAsync(
        int postId,
        [FromBody] CreateCommentDto dto)
    {
        try
        {
            await postRepository.GetSingleAsync(postId);
            User user = await userRepository.GetSingleAsync(dto.UserId);
            Comment comment = new Comment
            {
                Body = dto.Body,
                UserId = dto.UserId,
                PostId = postId,
                CreatedAt = DateTime.Now
            };
            Comment createdComment = await commentRepository.AddAsync(comment);
            CommentDto response = new CommentDto
            {
                Id = createdComment.Id,
                UserId = createdComment.UserId,
                Body = createdComment.Body,
                PostId = createdComment.PostId,
                UserName = user.UserName,
                CreatedAt = createdComment.CreatedAt
            };
            return Created(
                $"/api/comments/{createdComment.Id}",
                response
            );
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Post or user was not found.");
        }
    }
    [HttpGet("comments/{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingleAsync(int id)
    {
        try
        {
            Comment comment = await commentRepository.GetSingleAsync(id);
            User user = await userRepository.GetSingleAsync(comment.UserId);

            CommentDto response = new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                UserName = user.UserName,
                PostId = comment.PostId,
                CreatedAt = comment.CreatedAt
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with ID {id} was not found.");
        }
    }
    [HttpGet("comments")]
    public ActionResult<IEnumerable<CommentDto>> GetMany(
        [FromQuery] int? postId = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? userName = null)
    {
        var comments = commentRepository
            .GetMany()
            .AsEnumerable();

        if (postId is not null)
        {
            comments = comments.Where(comment =>
                comment.PostId == postId);
        }

        if (userId is not null)
        {
            comments = comments.Where(comment =>
                comment.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var matchingUserIds = userRepository
                .GetMany()
                .AsEnumerable()
                .Where(user =>
                    user.UserName.Contains(
                        userName,
                        StringComparison.OrdinalIgnoreCase))
                .Select(user => user.Id)
                .ToList();

            comments = comments.Where(comment =>
                matchingUserIds.Contains(comment.UserId));
        }

        var response = comments
            .Select(comment =>
            {
                User? user = userRepository
                    .GetMany()
                    .SingleOrDefault(user => user.Id == comment.UserId);

                return new CommentDto
                {
                    Id = comment.Id,
                    Body = comment.Body,
                    UserId = comment.UserId,
                    UserName = user?.UserName ?? "Unknown",
                    PostId = comment.PostId,
                    CreatedAt = comment.CreatedAt
                };
            })
            .ToList();

        return Ok(response);
    }
    [HttpPut("comments/{id:int}")]
    public async Task<ActionResult<CommentDto>> UpdateAsync(
        int id,
        [FromBody] UpdateCommentDto dto)
    {
        try
        {
            Comment comment = await commentRepository.GetSingleAsync(id);

            comment.Body = dto.Body;

            await commentRepository.UpdateAsync(comment);

            User user = await userRepository.GetSingleAsync(comment.UserId);

            CommentDto response = new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                UserName = user.UserName,
                PostId = comment.PostId,
                CreatedAt = comment.CreatedAt
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with ID {id} was not found.");
        }
    }
    [HttpDelete("comments/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            await commentRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with ID {id} was not found.");
        }
    }
}