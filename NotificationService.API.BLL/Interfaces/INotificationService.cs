namespace NotificationService.API.BLL.Interfaces;

public interface INotificationService
{
    Task EnqueueNotificationAsync(string email);
}