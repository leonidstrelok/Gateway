using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.API.BLL.Interfaces;
using NotificationService.API.BLL.Services;
using NotificationService.API.BLL.Workers;

namespace NotificationService.API.BLL;

public static class ServiceCollectionExtensions
{
    public static void AddBllServiceCollection(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<NotificationWorker>();
        services.AddScoped<INotificationService, Services.NotificationService>();

        services.AddFluentEmail("noreply@example.com", "Notification Service")
            .AddMailKitSender(new SmtpClientOptions
            {
                Server = "smtp.example.com",
                Port = 587,
                User = "username",
                Password = "password",
                RequiresAuthentication = true,
                UseSsl = true
            });
    }
}