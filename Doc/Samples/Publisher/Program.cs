using System;
using System.Threading.Tasks;
using KafkaMessageBus;
using KafkaMessageBus.Abstractions;

namespace Samples.Publisher
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // local
            // var brokers = new string[] { "localhost2:9092" };

            // tavana
            var brokers = new string[] { "192.168.34.104:9092" };

            // sedna
            // var brokers = new string[] { "91.99.98.201:9092" };
            // var brokers = new string[] { "91.99.98.202:9092" };

            var messageBus = new PublishMessageBus(brokers);

            var message = new TempMessage {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            var result = await messageBus.PublishAsync("SejamCountry-test", message);
            // Console.WriteLine();
        }
    }
}
