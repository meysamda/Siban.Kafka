using KafkaMessageBus;
using Samples.Messages;
using System.Threading.Tasks;

namespace Samples.Subscriber.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var brokers = new string[] { "localhost:9092" };
            var messageBus = new SubscriptionMessageBus(brokers);

            await messageBus.Subscribe<TempMessage>(
                new string[] { "test-topic" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );
        }
    }
}
