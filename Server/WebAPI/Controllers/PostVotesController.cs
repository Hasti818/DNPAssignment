using ApiContracts.PostVotes;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api")]
public class PostVotesController : ControllerBase
{
    private readonly IPostVoteRepository postVoteRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public PostVotesController(
        IPostVoteRepository postVoteRepository,
        IPostRepository postRepository,
        IUserRepository userRepository)
    {
        this.postVoteRepository = postVoteRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    // CREATE
    // POST /api/posts/3/votes
    [HttpPost("posts/{postId:int}/votes")]
    public async Task<ActionResult<PostVoteDto>> CreateAsync(
        int postId,
        [FromBody] CreatePostVoteDto dto)
    {
        try
        {
            await postRepository.GetSingleAsync(postId);
            await userRepository.GetSingleAsync(dto.UserId);

            PostVote? existingVote = postVoteRepository
                .GetMany()
                .SingleOrDefault(vote =>
                    vote.PostId == postId &&
                    vote.UserId == dto.UserId);

            if (existingVote is not null)
            {
                return BadRequest("User has already voted on this post.");
            }

            PostVote postVote = new PostVote
            {
                UserId = dto.UserId,
                PostId = postId,
                IsLike = dto.IsLike
            };

            PostVote createdVote =
                await postVoteRepository.AddAsync(postVote);

            PostVoteDto response = new PostVoteDto
            {
                Id = createdVote.Id,
                UserId = createdVote.UserId,
                PostId = createdVote.PostId,
                IsLike = createdVote.IsLike
            };

            return Created(
                $"/api/postvotes/{createdVote.Id}",
                response
            );
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Post or user was not found.");
        }
    }

    // GET ONE
    // GET /api/postvotes/3
    [HttpGet("postvotes/{id:int}")]
    public async Task<ActionResult<PostVoteDto>> GetSingleAsync(int id)
    {
        try
        {
            PostVote vote =
                await postVoteRepository.GetSingleAsync(id);

            PostVoteDto response = new PostVoteDto
            {
                Id = vote.Id,
                UserId = vote.UserId,
                PostId = vote.PostId,
                IsLike = vote.IsLike
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post vote with ID {id} was not found.");
        }
    }

    // GET ALL VOTES FOR A POST
    // GET /api/posts/3/votes
    [HttpGet("posts/{postId:int}/votes")]
    public ActionResult<IEnumerable<PostVoteDto>> GetMany(
        int postId,
        [FromQuery] int? userId = null,
        [FromQuery] bool? isLike = null)
    {
        var votes = postVoteRepository
            .GetMany()
            .Where(vote => vote.PostId == postId);

        if (userId is not null)
        {
            votes = votes.Where(vote => vote.UserId == userId);
        }

        if (isLike is not null)
        {
            votes = votes.Where(vote => vote.IsLike == isLike);
        }

        var response = votes
            .Select(vote => new PostVoteDto
            {
                Id = vote.Id,
                UserId = vote.UserId,
                PostId = vote.PostId,
                IsLike = vote.IsLike
            })
            .ToList();

        return Ok(response);
    }

    // UPDATE
    // PUT /api/postvotes/3
    [HttpPut("postvotes/{id:int}")]
    public async Task<ActionResult<PostVoteDto>> UpdateAsync(
        int id,
        [FromBody] UpdatePostVoteDto dto)
    {
        try
        {
            PostVote vote =
                await postVoteRepository.GetSingleAsync(id);

            vote.IsLike = dto.IsLike;

            await postVoteRepository.UpdateAsync(vote);

            PostVoteDto response = new PostVoteDto
            {
                Id = vote.Id,
                UserId = vote.UserId,
                PostId = vote.PostId,
                IsLike = vote.IsLike
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post vote with ID {id} was not found.");
        }
    }

    // DELETE
    // DELETE /api/postvotes/3
    [HttpDelete("postvotes/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            await postVoteRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post vote with ID {id} was not found.");
        }
    }
}