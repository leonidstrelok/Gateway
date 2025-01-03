using Microsoft.AspNetCore.Mvc;
using NotificationService.API.BLL.Interfaces;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        await notificationService.EnqueueNotificationAsync(request.Email);
        return Ok("Notification enqueued successfully");
    }
}

public record NotificationRequest(string Email);