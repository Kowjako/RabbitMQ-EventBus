using RabbitConsumer.EventBus;

namespace RabbitConsumer.EventBusSubManager
{
    public class Subscription
    {
        public Type EventType { get; private set; }
        public Type HandlerType { get; private set; }

        public Subscription(Type eventType, Type handlerType)
        {
            EventType = eventType;
            HandlerType = handlerType;
        }
    }

    public interface ISubscriptionManager
    {
        bool IsEmpty { get; }
        bool HasSubscriptionsForEvent(string eventName);
        void AddSubscription<TEvent, TEventHandler>() where TEvent : IEvent
                                                      where TEventHandler : IEventHandler<TEvent>;
        void RemoveSubscription<TEvent, TEventHandler>() where TEvent : IEvent
                                                         where TEventHandler : IEventHandler<TEvent>;

        IEnumerable<Subscription> GetHandlersForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
    }   
}
