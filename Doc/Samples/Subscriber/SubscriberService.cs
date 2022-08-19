using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using KafkaMessageBus.Abstractions;
using Samples.Messages;

namespace Samples.Subscriber
{
    public class SubscriberService : BackgroundService
    {
        private readonly ISubscriptionMessageBus _messageBus;
        private readonly GreetingMessageProcessor _processor;

        public SubscriberService(ISubscriptionMessageBus messageBus, GreetingMessageProcessor processor)
        {
            _messageBus = messageBus;
            processor = _processor;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _messageBus.SubscribeAsync<Greeting>(
                    new [] { "greeting" },
                    message => { 
                        Console.WriteLine(message.ToString());
                        return Task.CompletedTask;
                    },
                    // message => _processor.Process(message, cancellationToken),
                    options => {
                        options.ConsumerConfig.GroupId = "greeting";
                        options.ConsumerConfig.AllowAutoCreateTopics = true;
                        options.ConsumerConfig.EnableAutoCommit = false;
                        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                    },
                    cancellationToken
                );
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}
