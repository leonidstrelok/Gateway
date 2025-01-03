namespace MessageBroker.Configurations;

public class KafkaConfig
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string UserServiceNotificationTopic { get; set; } = string.Empty;
    public string ChatUserServiceTopic { get; set; } = string.Empty;
}