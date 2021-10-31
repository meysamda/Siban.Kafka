using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaMessageBus;

namespace Samples.Publisher.Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var bootstrapServers = new string[] { "localhost:9092" };
            var messageBus = new PublishMessageBus(bootstrapServers);

            messageBus.Publish("greeting-1", "hello world-1", options => {
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

            var result = await messageBus.PublishAsync("greeting-1", "hello world-2", options => {
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

            messageBus.Publish(producer, "greeting-1", "hello world-3", dr => { 
                if (dr.Error.IsError) {
                    System.Console.WriteLine(dr.Error.Reason);
                    throw new System.Exception($"error, {dr.Error.Reason}");
                }
            });

            result = await messageBus.PublishAsync(producer, "greeting-1", "hello world-4");
        }
    }
}