namespace MessageBroker.Interfaces;

public interface IMessageHandler
{
    Task HandleMessageAsync(object message);
}