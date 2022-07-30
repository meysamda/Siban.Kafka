using KafkaMessageBus.Abstractions;
using Microsoft.Extensions.Logging;
using Samples.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Subscriber
{
    public class GreetingMessageProcessor
    {
        private readonly ILogger<GreetingMessageProcessor> _logger;

        public GreetingMessageProcessor(ILogger<GreetingMessageProcessor> logger)
        {
            _logger = logger;
        }

        public Task Process(Greeting message, CancellationToken cancellationToken)
        {
            _logger.LogInformation(message.Body);
            return Task.CompletedTask;
        }
    }
}
