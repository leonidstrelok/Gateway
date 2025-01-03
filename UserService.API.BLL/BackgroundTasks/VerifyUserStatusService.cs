using MessageBroker.Configurations;
using MessageBroker.Interfaces;
using MessageBroker.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UserService.API.BLL.Interfaces;

namespace UserService.API.BLL.BackgroundTasks;

public class VerifyUserStatusService(IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var scope = services.CreateAsyncScope();
            var userStatusService = scope.ServiceProvider.GetRequiredService<IUserStatusService>();
            var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
            var kafkaOptions = scope.ServiceProvider.GetRequiredService<IOptions<KafkaConfig>>();
            var users = await userStatusService.GetAllOfflineUsersAsync();

            foreach (var userStatus in users)
            {
                await eventPublisher.PublishAsync(kafkaOptions.Value.UserServiceNotificationTopic, new NotificationDto(userStatus.Email));
            }
        }
    }
}