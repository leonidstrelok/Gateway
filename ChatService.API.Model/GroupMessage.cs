namespace ChatService.API.Model;

public class GroupMessage
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid GroupId { get; set; }

    public Message Message { get; set; }
    public Group Group { get; set; }
}