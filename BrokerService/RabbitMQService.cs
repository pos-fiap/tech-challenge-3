using MailKit.Net.Smtp;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BrokerService
{
    public class RabbitMQService
    {
        private readonly IConnection _cnn;
        private readonly IModel _channel;
        private readonly ConnectionFactory _factory;
        public RabbitMQService(string clientProvidedName)
        {
            //docker run -d --hostname rmq --name rabbit-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management

            _factory = new()
            {
                Uri = new Uri("amqp://guest:guest@rabbitmq:5672"),
                ClientProvidedName = clientProvidedName
            };

            _cnn = _factory.CreateConnection();
            _channel = _cnn.CreateModel();
        }
        public void SendEmailToQueue(EmailDto email)
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

        public void ReceiveEmailAndSend(string emailHost, string emailHostPassword)
        {
            string exchangeName = "email-exchange";
            string routingKey = "email-routing-key";
            string queueName = "email-queue";

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queueName, false, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routingKey, null);
            _channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                EmailDto email = JsonSerializer.Deserialize<EmailDto>(message);

                Console.WriteLine($"Email recebido: {email.To} - {email.Subject} - {email.Body}");

                try
                {
                    SendEmail(email, emailHost, emailHostPassword);
                }
                catch (Exception)
                {

                    throw;
                }

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(queueName, true, consumer);

            Console.ReadLine();

            _channel.Close();
            _cnn.Close();
        }

        private static void SendEmail(EmailDto email, string emailHost, string emailHostPassword)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(MailboxAddress.Parse(emailHost));
            mimeMessage.To.Add(MailboxAddress.Parse(email.To));
            mimeMessage.Subject = email.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = email.Body };
            var smtp = new SmtpClient();

            smtp.Connect("smtp-mail.outlook.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(emailHost, emailHostPassword);
            smtp.Send(mimeMessage);
            smtp.Disconnect(true);
            smtp.Dispose();
        }
    }
}
