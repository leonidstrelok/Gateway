using Confluent.Kafka;
using Confluent.Kafka.Admin;
using MessageBroker.Configurations;
using Microsoft.Extensions.Options;

namespace MessageBroker.Services;

public class CreateTopic(IOptions<KafkaConfig> options)
{
    public async Task CreateTopicAsync()
    {
        var config = new AdminClientConfig()
        {
            BootstrapServers = options.Value.BootstrapServers
        };

        using var adminClient = new AdminClientBuilder(config).Build();

        var topicSpecification = new List<TopicSpecification>()
        {
            new()
            {
                Name = options.Value.UserServiceNotificationTopic,
                NumPartitions = 3,
                ReplicationFactor = 1
            }
        };

        try
        {
            await adminClient.CreateTopicsAsync(topicSpecification);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}