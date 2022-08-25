using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using Siban.Kafka.Abstractions;

namespace Siban.Kafka.Samples.Subscriber
{
    public class SubscriberService : BackgroundService
    {
        private readonly ISubscriptionMessageBus _messageBus;
        private readonly GreetingMessageProcessor _processor;

        public SubscriberService(ISubscriptionMessageBus messageBus, GreetingMessageProcessor processor)
        {
            _messageBus = messageBus;
            _processor = processor;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _messageBus.SubscribeAsync<Greeting>(
                    new [] { "greeting-1" },
                    // (message, headers) => { 
                    //     Console.WriteLine(message.ToString());
                    //     return Task.CompletedTask;
                    // },
                    (message, headers) => _processor.Process(message, headers, cancellationToken),
                    options => {
                        options.ConsumerConfig.GroupId = "greeting-1";
                        options.ConsumerConfig.AllowAutoCreateTopics = true;
                        options.ConsumerConfig.EnableAutoCommit = false;
                        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
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
