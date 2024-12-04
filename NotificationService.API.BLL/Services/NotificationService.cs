using FluentEmail.Core;
using NotificationService.API.BLL.Interfaces;

namespace NotificationService.API.BLL.Services;

public class NotificationService(IBackgroundTaskQueue backgroundTaskQueue) : INotificationService
{
    public async Task EnqueueNotificationAsync(string userId, string email, string message)
    {
        await backgroundTaskQueue.QueueBackgroundWorkItemAsync(async _ =>
        {
            await SendEmailNotificationAsync(email, message);
        });
    }

    private async Task SendEmailNotificationAsync(string email, string message)
    {
        var emailSender = new Email("noreply@example.com")
            .To(email)
            .Subject("Unread Message Notification")
            .Body($"You have an unread message: {message}");

        await emailSender.SendAsync();
    }
}