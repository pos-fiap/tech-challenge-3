using BrokerService;

namespace EmailSender
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string _emailHost;
        private readonly string _emailHostPassword;
        private bool _disposed = false;
        public Worker(IConfiguration configuration)
        {
            _configuration = configuration;
            _emailHost = _configuration["emailHost"];
            _emailHostPassword = _configuration["emailHostPassword"];
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ProcessarEmails(cancellationToken);

            return Task.CompletedTask;
        }

        private void ProcessarEmails(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                RabbitMQService rabbitService = new RabbitMQService("RabbitMQ .NET 8 Sender App");
                rabbitService.ReceiveEmailAndSend(_emailHost, _emailHostPassword);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose de recursos
                }
                _disposed = true;
            }
        }
    }
}
