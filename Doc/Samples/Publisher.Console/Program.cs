using System;
using System.Threading.Tasks;
using KafkaMessageBus;
using Samples.Messages;

namespace Samples.Publisher.Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var brokers = new string[] { "localhost:9092" };
            var messageBus = new PublishMessageBus(brokers);

            var message = new TempMessage {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            var result = await messageBus.PublishAsync("test-topic", message);

            result = await messageBus.PublishAsync("test-topic", message, options =>
            {
                options.ProducerConfig.Acks = Confluent.Kafka.Acks.All;
                options.ProducerConfig.BootstrapServers = "some thing different from default brokers defined in message bus instantiating";
                options.ProducerConfig.MessageTimeoutMs = 50000;
            });
        }
    }
}