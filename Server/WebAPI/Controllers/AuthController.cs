using ApiContracts.Auth;
using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        User? user = userRepository
            .GetMany()
            .SingleOrDefault(user => user.UserName == request.Username);
        if (user is null)
        {
            return Unauthorized("invalid username");
        }

        if (user.Password != request.Password)
        {
            return Unauthorized("invalid password");
        }

        UserDto userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
        };
        return Ok(userDto);
    }
}