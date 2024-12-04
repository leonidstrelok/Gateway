using ChatService.API.BLL.DTOs;
using ChatService.API.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers;

[ApiController]
[Route("api/[controller]/messages")]
public class ChatController(IMessageService messageService) : ControllerBase
{
    [HttpGet("{userId}/{otherUserId}")]
    public async Task<IActionResult> GetMessages(string userId, string otherUserId)
    {
        var messages = await messageService.GetMessagesAsync(userId, otherUserId);
        return Ok(messages);
    }

    [HttpPut("")]
    public async Task<IActionResult> UpdateMessage([FromBody] UpdateMessageDto dto)
    {
        var result = await messageService.UpdateMessageAsync(dto.Id, dto.Content);
        if (!result) return NotFound();

        return Ok("Message updated");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid id)
    {
        var result = await messageService.DeleteMessageAsync(id);
        if (!result) return NotFound();

        return Ok("Message deleted");
    }
}