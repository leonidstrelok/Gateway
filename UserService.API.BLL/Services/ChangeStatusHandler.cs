using System.Text.Json;
using MessageBroker.Interfaces;
using MessageBroker.Models;
using Microsoft.Extensions.DependencyInjection;
using UserService.API.BLL.Interfaces;

namespace UserService.API.BLL.Services;

public class ChangeStatusHandler(IServiceProvider services) : IMessageHandler
{
    public async Task HandleMessageAsync(object message)
    {
        if (JsonSerializer.Deserialize(message.ToString()!,
                typeof(UserStatusDto)) is not UserStatusDto userStatus)
            throw new InvalidOperationException("Message it is not possible to deserialize to an object AuthDto");

        await using var scope = services.CreateAsyncScope();
        var userStatusService = scope.ServiceProvider.GetRequiredService<IUserStatusService>();
        await userStatusService.SetUserStatusAsync(userStatus.UserId, userStatus.IsOnline,
            userStatus.Email);
    }
}