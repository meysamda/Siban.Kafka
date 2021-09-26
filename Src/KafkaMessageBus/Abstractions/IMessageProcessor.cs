using System.Threading;
using System.Threading.Tasks;

namespace KafkaMessageBus.Abstractions
{
    public interface IMessageProcessor<in TMessage> where TMessage : IMessage
    {
        Task Process(TMessage message, CancellationToken cancellationToken);
    }
}
