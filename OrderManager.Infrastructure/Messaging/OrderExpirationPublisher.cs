using OrderManager.Infrastructure.Helper;
using RabbitMQ.Client;
using System.Text;
using OrderManager.Application.Interfaces;
using OrderManager.Infrastructure.Policies;

namespace OrderManager.Infrastructure.Messaging
{
    public class OrderExpirationPublisher : IOrderExpirationPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IReservationPolicy _policy;
        private readonly Dictionary<string, object> _args;

        public OrderExpirationPublisher(IReservationPolicy policy)
        {
            _policy = policy;
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };
            _args = new Dictionary<string, object>
            {
                { MessageNames.Exchange.DeadLetter, MessageNames.Exchange.OrderExpired }
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void PublishExpiration(int orderId)
        {
            _channel.QueueDeclare(
                queue: MessageNames.Queue.OrderExpiration,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: _args
            );

            _channel.ExchangeDeclare(MessageNames.Exchange.OrderExpired, ExchangeType.Fanout);
            _channel.QueueDeclare(MessageNames.Queue.OrderExpired, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(MessageNames.Queue.OrderExpired, MessageNames.Exchange.OrderExpired, "");

            var body = Encoding.UTF8.GetBytes(orderId.ToString());

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: MessageNames.Queue.OrderExpiration,
                basicProperties: GetBasicProps(),
                body: body
            );
        }

        private IBasicProperties GetBasicProps()
        {
            var expiration = _policy.GetReservationTime();
            var props = _channel.CreateBasicProperties();
            props.Expiration = ((int)expiration.TotalMilliseconds).ToString();
            return props;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
