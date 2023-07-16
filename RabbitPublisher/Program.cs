using RabbitConsumer.Concrete;
using RabbitConsumer.EventBus;

Console.WriteLine("Click to send msg to bus");
Console.ReadKey();

var bus = new RabbitMQBus();
bus.Publish(new CreateOrderEvent()
{
    Log = "Hello world"
});

Console.Read();

