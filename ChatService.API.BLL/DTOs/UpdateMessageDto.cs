namespace ChatService.API.BLL.DTOs;

public class UpdateMessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
}