using RabbitConsumer.Concrete;
using RabbitConsumer.EventBus;


var bus = new RabbitMQBus();
bus.Subscribe<SendMailEvent, SendMailEventHandler>();
bus.Subscribe<CreateOrderEvent, CreateOrderEventHandler>();

Console.ReadLine();



