using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OrderManager.Infrastructure.Helper;
using OrderManager.Application.Interfaces;

namespace OrderManager.Infrastructure.Messaging
{
    public class OrderExpirationConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IOrderService _orderService;
        private readonly IModel _channel;

        public OrderExpirationConsumer(IOrderService orderService)
        {
            _orderService = orderService;
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                Port = 5672 
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);

            _channel.QueueDeclare(queue: MessageNames.Queue.OrderExpired,
                durable: true,
                exclusive: false,
                autoDelete: false);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (int.TryParse(message, out int orderId))
                {
                    _orderService.ExpireOrder(orderId);
                }
                
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: MessageNames.Queue.OrderExpired,
                autoAck: false,
                consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}