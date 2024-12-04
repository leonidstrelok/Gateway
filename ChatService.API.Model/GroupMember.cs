namespace ChatService.API.Model;

public class GroupMember
{
    public Guid Id { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    public Guid GroupId { get; set; }

    public Group Group { get; set; }
}