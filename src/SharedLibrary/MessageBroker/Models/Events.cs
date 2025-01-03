using MessageBroker.Enums;

namespace MessageBroker.Models;

public record Events(EventType Event, object Data);