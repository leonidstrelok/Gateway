namespace UserService.API.Models;

public class UserStatus
{
    public required string UserId { get; set; } = string.Empty;
    public required bool IsOnline { get; set; }
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public required string Email { get; set; }
}