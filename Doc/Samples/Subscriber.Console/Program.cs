using KafkaMessageBus;
using Samples.Messages;
using System.Threading.Tasks;

namespace Samples.Subscriber.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bootstrapServers = new string[] { "localhost:9092" };
            var messageBus = new SubscriptionMessageBus(bootstrapServers);

            var t1 = messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t2 = messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options =>
                {
                    options.ConsumerConfig.GroupId = "new-group-1";
                    options.ConsumerConfig.BootstrapServers = "some thing different from default bootstrapServers defined in message bus registering phase";
                    options.ConsumerConfig.Acks = Confluent.Kafka.Acks.All;
                }
            );

            await Task.WhenAll(t1, t2);
        }
    }
}
