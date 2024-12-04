using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserService.API.BLL.Interfaces;
using UserService.API.Models;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/status")]
public class UserStatusController(IUserStatusService userStatusService) : ControllerBase
{
    [HttpPost("set")]
    public async Task<IActionResult> SetUserStatus([FromBody] UserStatus request)
    {
        await userStatusService.SetUserStatusAsync(request.UserId, request.IsOnline);
        return Ok("User status updated");
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserStatus(string userId)
    {
        var status = await userStatusService.GetUserStatusAsync(userId);
        if (status == null) return NotFound("User status not found");

        return Ok(status);
    }

    [HttpGet("online")]
    public async Task<IActionResult> GetAllOnlineUsers()
    {
        var users = await userStatusService.GetAllOnlineUsersAsync();
        return Ok(users);
    }
}