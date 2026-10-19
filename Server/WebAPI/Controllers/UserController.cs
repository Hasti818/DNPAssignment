using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    // CREATE USER
    // POST /api/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateAsync(
        [FromBody] CreateUserDto dto)
    {
        bool usernameTaken = userRepository
            .GetMany()
            .Any(user => user.UserName == dto.UserName);

        if (usernameTaken)
        {
            return BadRequest("Username is already taken.");
        }

        User user = new User
        {
            UserName = dto.UserName,
            Password = dto.Password
        };

        User createdUser = await userRepository.AddAsync(user);

        UserDto response = new UserDto
        {
            Id = createdUser.Id,
            UserName = createdUser.UserName
        };

        return CreatedAtAction(
            nameof(GetSingleAsync),
            new { id = createdUser.Id },
            response
        );
    }

    // GET ONE USER
    // GET /api/users/3
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingleAsync(int id)
    {
        try
        {
            User user = await userRepository.GetSingleAsync(id);

            UserDto response = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID {id} was not found.");
        }
    }
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany(
        [FromQuery] string? userName = null)
    {
        var users = userRepository
            .GetMany()
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(userName))
        {
            users = users.Where(user =>
                user.UserName.Contains(
                    userName,
                    StringComparison.OrdinalIgnoreCase));
        }

        var response = users
            .Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            })
            .ToList();

        return Ok(response);
    }
    // UPDATE USER
// PUT /api/users/3
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateAsync(
        int id,
        [FromBody] UpdateUserDto dto)
    {
        try
        {
            User user = await userRepository.GetSingleAsync(id);

            bool usernameTaken = userRepository
                .GetMany()
                .Any(existingUser =>
                    existingUser.UserName == dto.UserName &&
                    existingUser.Id != id);

            if (usernameTaken)
            {
                return BadRequest("Username is already taken.");
            }

            user.UserName = dto.UserName;
            user.Password = dto.Password;

            await userRepository.UpdateAsync(user);

            UserDto response = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID {id} was not found.");
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            await userRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID {id} was not found.");
        }
    }
}