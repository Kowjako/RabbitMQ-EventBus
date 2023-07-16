using RabbitConsumer.EventBus;

namespace RabbitConsumer.Concrete
{
    public class SendMailEventHandler : IEventHandler<SendMailEvent>
    {
        public async Task HandleAsync(SendMailEvent @event)
        {
            await Task.Delay(5000);
            Console.WriteLine($"Mail was sent = {@event.Body}");
        }
    }
}
