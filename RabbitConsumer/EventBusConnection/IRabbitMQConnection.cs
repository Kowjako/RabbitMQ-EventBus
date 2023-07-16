using RabbitMQ.Client;

namespace RabbitConsumer.EventBusConnection
{
    public interface IRabbitMQConnection
    {
        bool IsConnected {get;}
        bool TryConnect();
        IModel CreateModel();
    }
}
