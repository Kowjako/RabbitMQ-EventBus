namespace RabbitConsumer.EventBus
{
    public interface IEvent { }

    public abstract class BaseEvent : IEvent
    {
        public DateTime TimeStamp { get; protected set; } = DateTime.UtcNow;
    }
}
