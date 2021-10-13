﻿using System.Threading.Tasks;
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
            var message = new TempMessage
            {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            var result = await _messageBus.PublishAsync("test-topic", message);

            result = await _messageBus.PublishAsync("test-topic", message, options =>
            {
                options.ProducerConfig.Acks = Confluent.Kafka.Acks.All;
                options.ProducerConfig.BootstrapServers = "some thing different from default bootstrapServers defined in message bus registering phase";
                options.ProducerConfig.MessageTimeoutMs = 50000;
            });
        }
    }
}
