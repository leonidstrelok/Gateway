namespace ApplicationClient.Models;

public class GetAllOnlineUser
{
    public string UserId { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
}