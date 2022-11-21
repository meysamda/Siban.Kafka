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
        private readonly GreetingHandler _handler;

        public SubscriberService(ISubscriptionMessageBus messageBus, GreetingHandler handler)
        {
            _messageBus = messageBus;
            _handler = handler;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeForMessageValueAsync<OnlineCustomerChangeIntegrationEventMessage>(
                    new [] { "online-customer" },
                    (value) => 
                    {
                        Console.WriteLine(value.Content);
                        return Task.CompletedTask;
                    },
                    options => {
                        options.ConsumerConfig.GroupId = "cmanager";
                        options.ConsumerConfig.AllowAutoCreateTopics = true;
                        options.ConsumerConfig.EnableAutoCommit = false;
                        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                    },
                    cancellationToken
                );

            try
            {
                var t1 = _messageBus.SubscribeForMessageValueAsync<string>(
                    new [] { "greeting-1" },
                    (value) => _handler.Handle(value, cancellationToken),
                    options => {
                        options.ConsumerConfig.GroupId = "greeting";
                        options.ConsumerConfig.AllowAutoCreateTopics = true;
                        options.ConsumerConfig.EnableAutoCommit = false;
                        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
                    },
                    cancellationToken
                );

                var t2 = _messageBus.SubscribeForMessageValueAsync<Greeting>(
                    new [] { "greeting-2" },
                    (value) => _handler.Handle(value, cancellationToken),
                    options => {
                        options.ConsumerConfig.GroupId = "greeting";
                        options.ConsumerConfig.AllowAutoCreateTopics = true;
                        options.ConsumerConfig.EnableAutoCommit = false;
                        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
                    },
                    cancellationToken
                );

                var t3 = _messageBus.SubscribeForMessageAsync<string, Greeting>(
                    new [] { "greeting-3" },
                    (message) => _handler.Handle(message, cancellationToken),
                    options => {
                        options.ConsumerConfig.GroupId = "greeting";
                        options.ConsumerConfig.AllowAutoCreateTopics = true;
                        options.ConsumerConfig.EnableAutoCommit = false;
                        options.ConsumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
                    },
                    cancellationToken
                );

                await Task.WhenAll(t1, t2, t3);
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }

    public class OnlineCustomerChangeIntegrationEventMessage
    {
        public Guid EventId { get; set; }
        public string Content { get; set; }
        public string EventName { get; set; }
    }
}
