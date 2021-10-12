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
            try
            {
                await _messageBus.Subscribe<TempMessage>(
                    new string[] { "test-topic" },
                    message => { 
                        Console.WriteLine(message.Body);
                        return Task.CompletedTask;
                    },
                    default(CancellationToken));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
