using AuthService.API.BLL.DTOs;
using AuthService.API.BLL.Interfaces;
using AuthService.API.Model.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!await userService.RegisterAsync(request))
            return BadRequest("User already exists");

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userService.AuthenticateAsync(request.Username, request.Password);
        if (user == null) return Unauthorized("Invalid username or password");

        var token = tokenService.GenerateToken(user);
        return Ok(new { Token = token });
    }
}