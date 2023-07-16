namespace RabbitConsumer.EventBus
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : IEvent;
        void Subscribe<TEvent, TEventHandler>() where TEvent : IEvent
                                                where TEventHandler : IEventHandler<TEvent>;
    }
}
