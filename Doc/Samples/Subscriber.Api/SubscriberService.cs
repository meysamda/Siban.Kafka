using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using KafkaMessageBus.Abstractions;
using Samples.Messages;

namespace Samples.Subscriber.Api
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
            var t1 = _messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                }
            );

            var t2 = _messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            var t3 = _messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t4 = _messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            // ------

            var t5 = _messageBus.Subscribe<GreetingMessageProcessor>(new string[] { "greeting-1" });

            var t6 = _messageBus.Subscribe<GreetingMessageProcessor>(
                new string[] { "greeting-1" },
                options => { /*modify default options*/ }
            );

            var t7 = _messageBus.Subscribe<Greeting, GreetingMessageProcessor>(new string[] { "greeting-2" });

            var t8 = _messageBus.Subscribe<Greeting, GreetingMessageProcessor>(
                new string[] { "greeting-2" },
                options => { /*modify default options*/ }
            );

            await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7, t8);
        }
    }
}
