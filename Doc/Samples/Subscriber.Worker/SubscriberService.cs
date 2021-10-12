using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using KafkaMessageBus.Abstractions;
using Samples.Messages;

namespace Samples.Subscriber.Worker
{
    public class SubscriberService : BackgroundService
    {
        private readonly ISubscriptionMessageBus _messageBus;

        public SubscriberService(ISubscriptionMessageBus messageBus)
        {
            _messageBus = messageBus;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var t1 = _messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t2 = _messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options =>
                {
                    options.ConsumerConfig.GroupId = "new-group-1";
                    options.ConsumerConfig.BootstrapServers = "some thing different from default brokers defined in message bus registering phase";
                    options.ConsumerConfig.Acks = Confluent.Kafka.Acks.All;
                }
            );

            await Task.WhenAll(t1, t2);
        }
    }
}
