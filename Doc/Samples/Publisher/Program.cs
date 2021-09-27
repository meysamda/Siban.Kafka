using System;
using System.Threading.Tasks;
using KafkaMessageBus;
using KafkaMessageBus.Abstractions;

namespace Samples.Publisher
{
    class Program
    {
        public static void Main(string[] args)
        {
            var brokers = new string[] { "localhost:9092" };
            var messageBus = new PublishMessageBus(brokers);

            var message = new TempMessage {
                Body = "Hello world",
                Number = 1,
                MessageId = Guid.NewGuid()
            };

            messageBus.Publish(message);
        }
    }
}
