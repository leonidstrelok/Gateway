using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.API.BLL.Interfaces;

namespace NotificationService.API.BLL.Workers;

public class NotificationWorker(IBackgroundTaskQueue backgroundTaskQueue, ILogger<NotificationWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await backgroundTaskQueue.DequeueAsync(stoppingToken);

                if (workItem is not null)
                {
                    await workItem(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing notification task.");
            }
        }
    }
}