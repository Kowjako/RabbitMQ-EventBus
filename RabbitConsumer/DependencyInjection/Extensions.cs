using Microsoft.Extensions.DependencyInjection;
using RabbitConsumer.EventBus;
using RabbitConsumer.EventBusConnection;
using RabbitConsumer.EventBusSubManager;
using RabbitMQ.Client;

namespace RabbitConsumer.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services, string connectionUrl)
        {
            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>(factory =>
            {
                var connection = new ConnectionFactory()
                {
                    Uri = new Uri(connectionUrl),
                    DispatchConsumersAsync = true
                };

                return new RabbitMQConnection(connection);
            });
            services.AddSingleton<IEventBus, RabbitMQBus>();
            return services;
        }
    }
}
