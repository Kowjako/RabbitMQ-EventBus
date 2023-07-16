using RabbitConsumer.EventBus;
#nullable disable
namespace RabbitConsumer.EventBusSubManager
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly Dictionary<string, List<Subscription>> _handlers;
        private readonly List<Type> _events;

        public SubscriptionManager()
        {
            _handlers = new Dictionary<string, List<Subscription>>();
            _events = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();

        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventType = typeof(TEvent);
            var eventName = eventType.Name;
            var handlerType = typeof(TEventHandler);

            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<Subscription>());
            }

            if (_handlers[eventName].Any(p => p.HandlerType == handlerType))
            {
                throw new ArgumentException("Handler type already registered");
            }

            _handlers[eventName].Add(new Subscription(eventType, handlerType));

            if (!_events.Contains(eventType))
            {
                _events.Add(eventType);
            }
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _events.SingleOrDefault(p => p.Name == eventName);
        }

        public IEnumerable<Subscription> GetHandlersForEvent(string eventName)
        {
            return _handlers[eventName];
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return _handlers.ContainsKey(eventName);
        }

        public void RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            var handlerType = typeof(TEventHandler);

            if (!HasSubscriptionsForEvent(eventName)) return;

            var handler = _handlers[eventName].SingleOrDefault(p => p.HandlerType == handlerType);
            if (handler != null)
            {
                _handlers[eventName].Remove(handler);
            }

            if (_handlers[eventName].Any()) return;

            _handlers.Remove(eventName); // remove key with subscription list
            var eventToRemove = _events.SingleOrDefault(p => p.Name == eventName);
            if (eventToRemove != null)
            {
                _events.Remove(eventToRemove); // remove event
            }
        }
    }
}
