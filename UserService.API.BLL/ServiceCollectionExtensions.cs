using System.Text;
using MessageBroker.BackgroundServices;
using MessageBroker.Configurations;
using MessageBroker.Enums;
using MessageBroker.Interfaces;
using MessageBroker.Models;
using MessageBroker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using UserService.API.BLL.BackgroundTasks;
using UserService.API.BLL.Interfaces;
using UserService.API.BLL.Services;

namespace UserService.API.BLL;

public static class ServiceCollectionExtensions
{
    public static void AddBllServiceCollection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!));

        services.AddScoped<IUserStatusService, UserStatusService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };
        });
        services.AddHostedService<VerifyUserStatusService>();
        ConfigurationKafka(services, configuration);
    }

    private static void ConfigurationKafka(IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = configuration.GetSection("Kafka").Get<KafkaConfig>();

        services.AddSingleton<IMessageHandler, ChangeStatusHandler>();
        services.AddSingleton<ChangeStatusHandler>();
        services.AddSingleton<IEventPublisher, KafkaPublisher>();

        services.AddSingleton<IEventSubscriber>(
            sp =>
            {
                var handlers = new Dictionary<EventType, IMessageHandler>
                {
                    { EventType.ChangeOnlineUser, sp.GetRequiredService<ChangeStatusHandler>() },
                };
                return new KafkaSubscriber(kafkaConfig!.BootstrapServers, KafkaGroups.ChatUserGroup,
                    kafkaConfig!.UserServiceNotificationTopic, handlers);
            });
        services.AddHostedService<KafkaSubscriberHostedService>();
    }
}