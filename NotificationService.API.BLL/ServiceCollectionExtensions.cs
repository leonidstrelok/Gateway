using FluentEmail.MailKitSmtp;
using MessageBroker.BackgroundServices;
using MessageBroker.Configurations;
using MessageBroker.Enums;
using MessageBroker.Interfaces;
using MessageBroker.Models;
using MessageBroker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.API.BLL.Interfaces;
using NotificationService.API.BLL.Services;
using NotificationService.API.BLL.Workers;

namespace NotificationService.API.BLL;

public static class ServiceCollectionExtensions
{
    public static void AddBllServiceCollection(this IServiceCollection services, IConfiguration configuration)
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
        ConfigurationKafka(services, configuration);
    }

    private static void ConfigurationKafka(IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = configuration.GetSection("Kafka").Get<KafkaConfig>();

        services.AddSingleton<IMessageHandler, EmailMessageHandler>();
        services.AddSingleton<EmailMessageHandler>();

        services.AddSingleton<IEventSubscriber>(
            sp =>
            {
                var handlers = new Dictionary<EventType, IMessageHandler>
                {
                    { EventType.SendEmailMessage, sp.GetRequiredService<EmailMessageHandler>() },
                };
                return new KafkaSubscriber(kafkaConfig!.BootstrapServers, KafkaGroups.ChatNotificationGroup,
                    kafkaConfig!.UserServiceNotificationTopic, handlers);
            });
        services.AddHostedService<KafkaSubscriberHostedService>();
    }
}