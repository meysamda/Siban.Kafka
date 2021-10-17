using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using KafkaMessageBus.Abstractions;
using Samples.Messages;

namespace Samples.Publisher.Worker
{
    public class PublisherService : BackgroundService
    {
        private readonly IPublishMessageBus _messageBus;

        public PublisherService(IPublishMessageBus messageBus)
        {
            _messageBus = messageBus;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _messageBus.Publish("greeting-1", "hello world");
            _messageBus.Publish("greeting-1", "hello world", options => { /* modify default options */ });
            await _messageBus.PublishAsync("greeting-1", "hello world");
            await _messageBus.PublishAsync("greeting-1", "hello world", options => { /* modify default options */ });
            
            var message = new Greeting { Body = "hello world" };

            _messageBus.Publish("greeting-2", message);
            _messageBus.Publish("greeting-2", message, options => { /* modify default options */ });
            await _messageBus.PublishAsync("greeting-2", message);
            await _messageBus.PublishAsync("greeting-2", message, options => { /* modify default options */ });
        }
    }
}
