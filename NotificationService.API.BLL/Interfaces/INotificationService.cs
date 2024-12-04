namespace NotificationService.API.BLL.Interfaces;

public interface INotificationService
{
    Task EnqueueNotificationAsync(string userId, string email, string message);
}