using ApiContracts.CommentVotes;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api")]
public class CommentVotesController : ControllerBase
{
    private readonly ICommentVoteRepository commentVoteRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public CommentVotesController(
        ICommentVoteRepository commentVoteRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        this.commentVoteRepository = commentVoteRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    // CREATE
    // POST /api/comments/3/votes
    [HttpPost("comments/{commentId:int}/votes")]
    public async Task<ActionResult<CommentVoteDto>> CreateAsync(
        int commentId,
        [FromBody] CreateCommentVoteDto dto)
    {
        try
        {
            await commentRepository.GetSingleAsync(commentId);
            await userRepository.GetSingleAsync(dto.UserId);

            CommentVote? existingVote = commentVoteRepository
                .GetMany()
                .SingleOrDefault(vote =>
                    vote.CommentId == commentId &&
                    vote.UserId == dto.UserId);

            if (existingVote is not null)
            {
                return BadRequest("User has already voted on this comment.");
            }

            CommentVote commentVote = new CommentVote
            {
                UserId = dto.UserId,
                CommentId = commentId,
                IsLike = dto.IsLike
            };

            CommentVote createdVote =
                await commentVoteRepository.AddAsync(commentVote);

            CommentVoteDto response = new CommentVoteDto
            {
                Id = createdVote.Id,
                UserId = createdVote.UserId,
                CommentId = createdVote.CommentId,
                IsLike = createdVote.IsLike
            };

            return Created(
                $"/api/commentvotes/{createdVote.Id}",
                response
            );
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Comment or user was not found.");
        }
    }

    // GET ONE
    // GET /api/commentvotes/3
    [HttpGet("commentvotes/{id:int}")]
    public async Task<ActionResult<CommentVoteDto>> GetSingleAsync(int id)
    {
        try
        {
            CommentVote vote =
                await commentVoteRepository.GetSingleAsync(id);

            CommentVoteDto response = new CommentVoteDto
            {
                Id = vote.Id,
                UserId = vote.UserId,
                CommentId = vote.CommentId,
                IsLike = vote.IsLike
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment vote with ID {id} was not found.");
        }
    }

    // GET ALL VOTES FOR A COMMENT
    // GET /api/comments/3/votes
    [HttpGet("comments/{commentId:int}/votes")]
    public ActionResult<IEnumerable<CommentVoteDto>> GetMany(
        int commentId,
        [FromQuery] int? userId = null,
        [FromQuery] bool? isLike = null)
    {
        var votes = commentVoteRepository
            .GetMany()
            .Where(vote => vote.CommentId == commentId);

        if (userId is not null)
        {
            votes = votes.Where(vote => vote.UserId == userId);
        }

        if (isLike is not null)
        {
            votes = votes.Where(vote => vote.IsLike == isLike);
        }

        var response = votes
            .Select(vote => new CommentVoteDto
            {
                Id = vote.Id,
                UserId = vote.UserId,
                CommentId = vote.CommentId,
                IsLike = vote.IsLike
            })
            .ToList();

        return Ok(response);
    }
    // UPDATE
    // PUT /api/commentvotes/3
    [HttpPut("commentvotes/{id:int}")]
    public async Task<ActionResult<CommentVoteDto>> UpdateAsync(
        int id,
        [FromBody] UpdateCommentVoteDto dto)
    {
        try
        {
            CommentVote vote =
                await commentVoteRepository.GetSingleAsync(id);

            vote.IsLike = dto.IsLike;

            await commentVoteRepository.UpdateAsync(vote);

            CommentVoteDto response = new CommentVoteDto
            {
                Id = vote.Id,
                UserId = vote.UserId,
                CommentId = vote.CommentId,
                IsLike = vote.IsLike
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment vote with ID {id} was not found.");
        }
    }

    // DELETE
    // DELETE /api/commentvotes/3
    [HttpDelete("commentvotes/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            await commentVoteRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment vote with ID {id} was not found.");
        }
    }
}