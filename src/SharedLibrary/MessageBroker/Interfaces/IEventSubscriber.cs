namespace MessageBroker.Interfaces;

public interface IEventSubscriber
{
    Task StartAsync(CancellationToken cancellationToken);
}