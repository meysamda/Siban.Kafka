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

            var t1 = messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                }
            );

            var t2 = messageBus.Subscribe(
                new string[] { "greeting-1" },
                message =>
                {
                    System.Console.WriteLine(message);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            var t3 = messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                }
            );

            var t4 = messageBus.Subscribe<Greeting>(
                new string[] { "greeting-2" },
                message =>
                {
                    System.Console.WriteLine(message.Body);
                    return Task.CompletedTask;
                },
                options => { /*modify default options*/ }
            );

            // ------

            var t5 = messageBus.Subscribe<GreetingMessageProcessor>(new string[] { "greeting-1" });

            var t6 = messageBus.Subscribe<GreetingMessageProcessor>(
                new string[] { "greeting-1" },
                options => { /*modify default options*/ }
            );

            var t7 = messageBus.Subscribe<Greeting, GreetingMessageProcessor>(new string[] { "greeting-2" });

            var t8 = messageBus.Subscribe<Greeting, GreetingMessageProcessor>(
                new string[] { "greeting-2" },
                options => { /*modify default options*/ }
            );

            await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7, t8);
        }
    }
}
