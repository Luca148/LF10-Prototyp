using LF10.Api.Models;
using LF10.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LF10.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] LoginModel model)
    {
        User user = new();
        if (!user.Register(model.Username, model.Password))
        {
            return Conflict("Username already exists.");
        }

        return Ok();
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        User user = new();
        if (!user.VerifyUsername(model.Username) || !user.VerifyPassword(model.Password))
        {
            return Unauthorized();
        }

        return Ok();
    }
}
