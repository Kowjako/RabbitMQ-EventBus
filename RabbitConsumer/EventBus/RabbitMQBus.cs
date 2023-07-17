using RabbitConsumer.EventBusConnection;
using RabbitConsumer.EventBusSubManager;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

#nullable disable
namespace RabbitConsumer.EventBus
{
    public class RabbitMQBus : IEventBus
    {
        private readonly IRabbitMQConnection _conn;
        private readonly ISubscriptionManager _mngr;
        private IModel _consumerChannel;

        public RabbitMQBus()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = AmqpTcpEndpoint.UseDefaultPort,
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };

            _conn = new RabbitMQConnection(factory);
            _mngr = new SubscriptionManager();
            _consumerChannel = CreateConsumerChannel();
        }

        private IModel CreateConsumerChannel()
        {
            var channel = _conn.CreateModel();
            channel.ExchangeDeclare("ex.default", "direct");
            channel.QueueDeclare("q.event-bus", true, false, false, null);

            return channel;
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            if (!_conn.IsConnected)
            {
                _conn.TryConnect();
            }

            var eventName = @event.GetType().Name;

            using var channel = _conn.CreateModel();
            channel.ExchangeDeclare("ex.default", "direct");

            var msg = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(msg);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish("ex.default", eventName, props, body);
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;

            CreateQueueBindingIfNotExist(eventName);
            _mngr.AddSubscription<TEvent, TEventHandler>();
            StartBasicConsume();
        }

        private void CreateQueueBindingIfNotExist(string eventName)
        {
            var anySub = _mngr.HasSubscriptionsForEvent(eventName);
            if (!anySub)
            {
                if (!_conn.IsConnected)
                {
                    _conn.TryConnect();
                }
                using var channel = _conn.CreateModel();
                channel.QueueBind("q.event-bus", "ex.default", eventName);
            }
        }

        private void StartBasicConsume()
        {
            if (_consumerChannel == null)
            {
                throw new ArgumentNullException(nameof(_consumerChannel));
            }

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            consumer.Received += HandleMessage;

            _consumerChannel.BasicConsume("q.event-bus", false, consumer);
        }

        private async Task HandleMessage(object sender, BasicDeliverEventArgs args)
        {
            var eventName = args.RoutingKey;
            var msg = Encoding.UTF8.GetString(args.Body.Span);

            var isAcked = false;

            try
            {
                await ProcessMessage(eventName, msg);
                _consumerChannel.BasicAck(args.DeliveryTag, false);
                isAcked = true;
            }
            finally
            {
                if (!isAcked)
                {
                    _consumerChannel.BasicNack(args.DeliveryTag, false, true);
                }
            }
        }

        private async Task ProcessMessage(string eventName, string msg)
        {
            if (!_mngr.HasSubscriptionsForEvent(eventName)) return;

            var subs = _mngr.GetHandlersForEvent(eventName);
            foreach(var singleSub in subs)
            {
                var handler = Activator.CreateInstance(singleSub.HandlerType);

                var eventType = _mngr.GetEventTypeByName(eventName);
                var @event  =JsonSerializer.Deserialize(msg, eventType);

                var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(singleSub.EventType);
                await (Task)eventHandlerType.GetMethod(nameof(IEventHandler<IEvent>.HandleAsync))
                                            .Invoke(handler, new object[] { @event });
            }
        }
    }
}
