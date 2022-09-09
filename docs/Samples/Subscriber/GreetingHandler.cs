using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Siban.Kafka.Samples.Subscriber
{
    public class GreetingHandler
    {
        private readonly ILogger<GreetingHandler> _logger;

        public GreetingHandler(ILogger<GreetingHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(string greeting, CancellationToken cancellationToken)
        {
            _logger.LogInformation(greeting);
            return Task.CompletedTask;
        }

        public Task Handle(Greeting greeting, CancellationToken cancellationToken)
        {
            _logger.LogInformation(greeting.Body);
            return Task.CompletedTask;
        }

        public Task Handle(Message<string, Greeting> message, CancellationToken cancellationToken)
        {
            _logger.LogInformation(message.Value.Body);
            return Task.CompletedTask;
        }
    }
}
