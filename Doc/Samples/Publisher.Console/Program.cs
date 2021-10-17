using System.Threading.Tasks;
using KafkaMessageBus;
using Samples.Messages;

namespace Samples.Publisher.Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var bootstrapServers = new string[] { "localhost:9092" };
            var messageBus = new PublishMessageBus(bootstrapServers);

            messageBus.Publish("greeting-1", "hello world");
            messageBus.Publish("greeting-1", "hello world", options => { /* modify default options */ });
            await messageBus.PublishAsync("greeting-1", "hello world");
            await messageBus.PublishAsync("greeting-1", "hello world", options => { /* modify default options */ });
            
            var message = new Greeting { Body = "hello world" };

            messageBus.Publish("greeting-2", message);
            messageBus.Publish("greeting-2", message, options => { /* modify default options */ });
            await messageBus.PublishAsync("greeting-2", message);
            await messageBus.PublishAsync("greeting-2", message, options => { /* modify default options */ });
        }
    }
}