using RabbitConsumer.EventBus;

namespace RabbitConsumer.Concrete
{
    public class CreateOrderEvent : BaseEvent
    {
        public string Log { get; set; }
    }
}
