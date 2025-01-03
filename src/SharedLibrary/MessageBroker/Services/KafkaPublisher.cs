using System.Text.Json;
using Confluent.Kafka;
using MessageBroker.Configurations;
using MessageBroker.Interfaces;
using Microsoft.Extensions.Options;

namespace MessageBroker.Services;

public class KafkaPublisher : IEventPublisher
{
    private readonly IProducer<Null, string> _producer;

    public KafkaPublisher(IOptions<KafkaConfig> options)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var kafkaMessage = new Message<Null, string> { Value = messageJson };

        await _producer.ProduceAsync(topic, kafkaMessage);
    }
}