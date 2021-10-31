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
            _messageBus.Publish("greeting-1", "hello world-1", options => {
                    options.ProducerConfig.ClientId = Dns.GetHostName();
                    options.ProducerConfig.Acks = Acks.Leader;
                    options.ProducerConfig.MessageTimeoutMs = 1000;
                },
                dr => {
                    if (dr.Error.IsError) {
                        System.Console.WriteLine(dr.Error.Reason);
                        throw new System.Exception($"error, {dr.Error.Reason}");
                    }
                });

            var result = await _messageBus.PublishAsync("greeting-1", "hello world-2", options => {
                    options.ProducerConfig.ClientId = Dns.GetHostName();
                    options.ProducerConfig.Acks = Acks.Leader;
                    options.ProducerConfig.MessageTimeoutMs = 1000;
                });

            var producerConfig = new ProducerConfig {
                BootstrapServers = "localhost:9092"
            };
            var producer = new ProducerBuilder<string, string>(producerConfig)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            _messageBus.Publish(producer, "greeting-1", "hello world-3", dr => { 
                if (dr.Error.IsError) {
                    System.Console.WriteLine(dr.Error.Reason);
                    throw new System.Exception($"error, {dr.Error.Reason}");
                }
            });

            result = await _messageBus.PublishAsync(producer, "greeting-1", "hello world-4");
        }
    }
}
