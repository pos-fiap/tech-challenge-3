using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace MinimalEmail
{
    public class RabbitMQSender
    {
        private readonly IConnection _cnn;
        private readonly IModel _channel;
        public RabbitMQSender()
        {
            //docker run -d --hostname rmq --name rabbit-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management

            ConnectionFactory factory = new();
            factory.Uri = new Uri("amqp://guest:guest@rabbitmq:5672");
            factory.ClientProvidedName = "RabbitMQ .NET 8 Sender App";

            _cnn = factory.CreateConnection();
            _channel = _cnn.CreateModel();
        }
        public void SendEmail(EmailDto email)
        {
            string exchangeName = "email-exchange";
            string routingKey = "email-routing-key";
            string queueName = "email-queue";

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queueName, false, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routingKey, null);

            byte[] messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(email));
            _channel.BasicPublish(exchangeName, routingKey, null, messageBody);

            _channel.Close();
            _cnn.Close();
        }
    }
}
