using RabbitMQ.Client;

#nullable disable
namespace RabbitConsumer.EventBusConnection
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _factory;
        private IConnection _connection;

        public RabbitMQConnection(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public bool TryConnect()
        {
            try
            {
                _connection = _factory.CreateConnection();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                TryConnect();
            }
            return _connection.CreateModel();
        }
    }
}
