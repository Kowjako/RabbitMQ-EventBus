using RabbitConsumer.EventBus;

namespace RabbitConsumer.Concrete
{
    public class CreateOrderEventHandler : IEventHandler<CreateOrderEvent>
    {
        public async Task HandleAsync(CreateOrderEvent @event)
        {
            await Task.Delay(3000);
            Console.WriteLine($"Order was created with product = {@event.Log}");
        }
    }
}
