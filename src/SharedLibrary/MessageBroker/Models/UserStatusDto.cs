namespace MessageBroker.Models;

public record UserStatusDto(string UserId, bool IsOnline, string Email);