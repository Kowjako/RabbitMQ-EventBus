using RabbitConsumer.EventBus;

namespace RabbitConsumer.Concrete
{
    public class SendMailEvent : BaseEvent
    {
        public string Body { get; set; }
    }
}
