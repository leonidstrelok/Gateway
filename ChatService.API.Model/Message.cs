namespace ChatService.API.Model;

public class Message
{
    public Guid Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public Guid? GroupId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }

    public Group Group { get; set; }
}