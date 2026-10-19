using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
namespace WebAPI.Controllers;
[ApiController]
[Route("api/posts")]
public class PostsController:ControllerBase
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    public PostsController(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreateAsync([FromBody] CreatePostDto dto)
    {
        try
        {
            User user =await userRepository.GetSingleAsync(dto.UserId);
            Post post = new Post
            {
                Title = dto.Title,
                Body = dto.Body,
                CreatedAt = DateTime.Now,
                UserId = dto.UserId,
            };
            Post createdPost= await postRepository.AddAsync(post);
            PostDto response = new PostDto
            {
                Id = createdPost.Id,
                Title = createdPost.Title,
                Body = createdPost.Body,
                UserId = createdPost.UserId,
                UserName = user.UserName,
                CreatedAt = createdPost.CreatedAt,
            };
            return CreatedAtAction(nameof(GetSingleAsync),
                new{id = createdPost.Id},response);
        }
        catch(InvalidOperationException e)
        {
            return BadRequest($"User with ID {dto.UserId} was not found.");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSingleAsync(int id)
    {
        try
        {
            Post post = await postRepository.GetSingleAsync(id);
            User user = await userRepository.GetSingleAsync(post.UserId);
            PostDto response = new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                UserName = user.UserName,
            };
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"Post with ID {id} was not found.");
        }
    }
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany(
        [FromQuery] string? title = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? userName = null)
    {
        var posts = postRepository
            .GetMany()
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(post =>
                post.Title.Contains(
                    title,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (userId is not null)
        {
            posts = posts.Where(post => post.UserId == userId);
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

            posts = posts.Where(post =>
                matchingUserIds.Contains(post.UserId));
        }

        var response = posts
            .Select(post =>
            {
                User? user = userRepository
                    .GetMany()
                    .SingleOrDefault(user => user.Id == post.UserId);

                return new PostDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    UserId = post.UserId,
                    UserName = user?.UserName ?? "Unknown",
                    CreatedAt = post.CreatedAt
                };
            })
            .ToList();

        return Ok(response);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdateAsync(
        int id,
        [FromBody] UpdatePostDto dto)
    {
        try
        {
            Post post = await postRepository.GetSingleAsync(id);
            post.Title = dto.Title;
            post.Body = dto.Body;
            await postRepository.UpdateAsync(post);
            User user = await userRepository.GetSingleAsync(post.UserId);
            PostDto response = new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId,
                UserName = user.UserName,
                CreatedAt = post.CreatedAt
            };
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"Post with ID {id} was not found.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            await postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with ID {id} was not found.");
        }
    }
}