using System.Text.Json;
using MessageBroker.Interfaces;
using MessageBroker.Models;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.API.BLL.Interfaces;

namespace NotificationService.API.BLL.Services;

public class EmailMessageHandler(IServiceProvider services) : IMessageHandler
{
    public async Task HandleMessageAsync(object message)
    {
        if (JsonSerializer.Deserialize(message.ToString()!,
                typeof(NotificationDto)) is not NotificationDto notification)
            throw new InvalidOperationException("Message it is not possible to deserialize to an object AuthDto");

        await using var scope = services.CreateAsyncScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        await notificationService.EnqueueNotificationAsync(notification.Email);
    }
}