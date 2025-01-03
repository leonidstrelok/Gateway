using System.Text.Json;
using Confluent.Kafka;
using MessageBroker.Enums;
using MessageBroker.Interfaces;
using MessageBroker.Models;

namespace MessageBroker.Services;

public class KafkaSubscriber : IEventSubscriber
{
    private readonly ConsumerConfig _config;
    private readonly string _topic;
    private readonly Dictionary<EventType, IMessageHandler> _handlers;

    public KafkaSubscriber(string bootstrapServers, string groupId, string topic,
        Dictionary<EventType, IMessageHandler> handlers)
    {
        ArgumentException.ThrowIfNullOrEmpty(bootstrapServers);
        ArgumentException.ThrowIfNullOrEmpty(groupId);
        ArgumentException.ThrowIfNullOrEmpty(topic);

        _config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            SessionTimeoutMs = 30000, // 30 секунд
            MaxPollIntervalMs = 300000, // 5 минут
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        _topic = topic;
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();

        consumer.Subscribe(_topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
                if (consumeResult is null) continue;

                var eventMessage = JsonSerializer.Deserialize<Events>(consumeResult.Message.Value);
                if (eventMessage is not null && _handlers.TryGetValue(eventMessage.Event, out var handler))
                {
                    await handler.HandleMessageAsync(eventMessage.Data);
                }
                else
                {
                    Console.WriteLine($"No handler found for event: {eventMessage?.Event}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}