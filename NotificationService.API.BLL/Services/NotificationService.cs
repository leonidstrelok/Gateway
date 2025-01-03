using FluentEmail.Core;
using NotificationService.API.BLL.Interfaces;

namespace NotificationService.API.BLL.Services;

public class NotificationService(IBackgroundTaskQueue backgroundTaskQueue) : INotificationService
{
    public async Task EnqueueNotificationAsync(string email)
    {
        await backgroundTaskQueue.QueueBackgroundWorkItemAsync(async _ =>
        {
            await SendEmailNotificationAsync(email);
        });
    }

    private async Task SendEmailNotificationAsync(string email)
    {
        var emailSender = new Email("noreply@example.com")
            .To(email)
            .Subject("Unread Message Notification")
            .Body($"You have an unread message");

        await emailSender.SendAsync();
    }
}