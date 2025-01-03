using MessageBroker.Interfaces;
using Microsoft.Extensions.Hosting;

namespace MessageBroker.BackgroundServices;

public class KafkaSubscriberHostedService(IEventSubscriber eventSubscriber) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await eventSubscriber.StartAsync(stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}