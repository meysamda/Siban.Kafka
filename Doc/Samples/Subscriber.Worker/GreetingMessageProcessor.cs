using KafkaMessageBus.Abstractions;
using Samples.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Subscriber.Worker
{
    public class GreetingMessageProcessor : IMessageProcessor<Greeting>, IMessageProcessor<string>
    {
        public Task Process(Greeting message, CancellationToken cancellationToken)
        {
            Console.WriteLine(message.Body);
            return Task.CompletedTask;
        }

        public Task Process(string message, CancellationToken cancellationToken)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
